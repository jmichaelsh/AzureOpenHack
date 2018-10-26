using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Backend_API_Minecraft.Model;
using Backend_API_Minecraft.Attributes;
using Swashbuckle.AspNetCore.Annotations;

using Backend_API_Minecraft.Builder;
using System.Net.Http;

using k8s;
using k8s.Models;
using Backend_API_Minecraft.Factory;
using System.Diagnostics.Contracts;

namespace Backend_API_Minecraft.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {

        [HttpDelete]
        [Route("//servers")]
        [SwaggerOperation("ServersDelete")]
        [SwaggerResponse(statusCode: 204, type: typeof(V1Status), description: "Apaga um Server")]
        public virtual IActionResult ServersDelete(string servername)
        {
            try
            {
                var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
                IKubernetes client = new Kubernetes(config);

                var v1Status = client.DeleteNamespacedDeployment(body: new V1DeleteOptions(apiVersion: "apps/v1"), name: servername, namespaceParameter: "default");

                return new ObjectResult(v1Status);

            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }


        private async Task<String> DeleteAKSAsync(string servername, string token)
        {
            var subscriptionID = "b9049f27-f15e-41ae-8853-8bc37dce9630";
            var resourceGroup = "desafio2";

            var requestUri = String.Format("https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.ContainerService/managedClusters/{2}?api-version=2018-03-31",
                                           subscriptionID, resourceGroup, servername);

            var response = await HttpRequestFactory.Delete(requestUri, token);

            return response.StatusCode.ToString();
        }


        private async Task<String> CreateAKSAsync(string servername, string token)
        {
            var subscriptionID = "b9049f27-f15e-41ae-8853-8bc37dce9630";
            var resourceGroup = "desafio2";

            var requestUri = String.Format("https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.ContainerService/managedClusters/{2}?api-version=2018-03-31",
                                           subscriptionID, resourceGroup, servername);

            var response = await HttpRequestFactory.Put(requestUri, token);

            return response.StatusCode.ToString();
        }


        [HttpGet]
        [Route("//servers")]
        [ValidateModelState]
        [SwaggerOperation("ServersGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Server>), description: "retorna lista de Servers")]
        public virtual IActionResult ServersGet()
        {
            try{

                return new ObjectResult(GetServers());
            }
            catch(Exception ex)
            {
                return new ObjectResult(ex.InnerException);
            }
        }


        private static List<Server>GetServers()
        {

            List<Server> servers = new List<Server>();

            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(config);
            Console.WriteLine("Starting Request!");

            var listDeploy = client.ListNamespacedDeployment("default");
            var listServices = client.ListNamespacedService("default");




            foreach (var deploy in listDeploy.Items)
            {

                string ipExternal = "";


                var service = listServices.Items.Where(a => a.Metadata.Name == deploy.Metadata.Name).FirstOrDefault();

                if (service != null)
                {
                    if (service.Status.LoadBalancer != null && service.Status.LoadBalancer.Ingress != null && service.Status.LoadBalancer.Ingress.Count()> 0)
                        ipExternal = service.Status.LoadBalancer.Ingress.First().Ip;
                     

                    string exampleJson = null;
                    exampleJson = "{\n  \"endpoints\" : {\n    \"minecraft\" : \"minecraft\",\n    \"rcon\" : \"rcon\"\n  },\n  \"name\" : \"name\"\n}";

                    var server = exampleJson != null
                    ? JsonConvert.DeserializeObject<Server>(exampleJson)
                    : default(Server);

                    server.Name = deploy.Metadata.Name;
                    server.Endpoints.Minecraft = ipExternal + ":25565";
                    server.Endpoints.Rcon = ipExternal + ":25575";

                    servers.Add(server);

                }
            }

            return servers;
        }


        [HttpPost]
        [Route("//servers")]
        [ValidateModelState]
        [SwaggerOperation("ServersPost")]
        [SwaggerResponse(statusCode: 201, type: typeof(V1Deployment), description: "Cria um novo Server")]
        public virtual IActionResult ServersPost(string servername)
        {
            Contract.Ensures(Contract.Result<IActionResult>() != null);
            try
            {

                var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
                IKubernetes client = new Kubernetes(config);



                var deployment = ObterYaml<V1Deployment>(servername, "./aks-minecraft.yaml");

                var services = ObterYaml<V1Service>(servername, "./services.yaml");


                //yaml.Metadata.Name = servername;
                //yaml.Spec.Selector.MatchLabels["app"] = servername;
                //yaml.Spec.Template.Metadata.Labels["app"] = servername;
                //yaml.Spec.Template.Spec.Containers.First().Name = servername;
                //yaml.Spec.Template.Spec.Containers.First().VolumeMounts.First().SubPath = servername;



                var newDeployment = client.CreateNamespacedDeployment(
                    body: deployment, namespaceParameter: "default");

                var newServices = client.CreateNamespacedService(
                    body: services, namespaceParameter: "default");


                string exampleJson = null;
                exampleJson = "{\n  \"endpoints\" : {\n    \"minecraft\" : \"minecraft\",\n    \"rcon\" : \"rcon\"\n  },\n  \"name\" : \"name\"\n}";


                var example = exampleJson != null
                ? JsonConvert.DeserializeObject<Server>(exampleJson)
                : default(Server);

             

                return new ObjectResult(newDeployment);
            }
            catch(Exception ex)
            {
                return new ObjectResult(ex.Message);
            }
        }

        private T ObterYaml<T>(string name, string ymalfile)
        {
            var file = System.IO.File.ReadAllText(ymalfile);
            var fileUpdate = file.Replace("azure-minecraft-server", name.ToLower());
           

            return Yaml.LoadFromString<T>(fileUpdate);
        }
    }
}
