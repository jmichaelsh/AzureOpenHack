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
        public virtual IActionResult ServersDelete(string servername, string token)
        {
            try
            {
                //var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
                //IKubernetes client = new Kubernetes(config);

                //var v1Status = client.DeleteNamespacedPod(body: new V1DeleteOptions(apiVersion: "apps/v1"), name: servername, namespaceParameter: "default");

                //return new ObjectResult(v1Status);

                var response = DeleteAKSAsync(servername, token);
                return new ObjectResult(response);

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

        [HttpGet]
        [Route("//servers")]
        [ValidateModelState]
        [SwaggerOperation("ServersGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Server>), description: "retorna lista de Servers")]
        public virtual IActionResult ServersGet()
        {
            List<Server> pods = new List<Server>();

            var vpods = GetPod();

            foreach (var vpod in vpods)
            {

                string exampleJson = null;
                exampleJson = "{\n  \"endpoints\" : {\n    \"minecraft\" : \"minecraft\",\n    \"rcon\" : \"rcon\"\n  },\n  \"name\" : \"name\"\n}";


                var example = exampleJson != null
                ? JsonConvert.DeserializeObject<Server>(exampleJson)
                : default(Server);

                example.Name = vpod.Key;
                example.Endpoints.Minecraft = vpod.Value;

                pods.Add(example);

            }

            return new ObjectResult(pods);
        }


        private static Dictionary<String, String> GetPod()
        {

            Dictionary<String, String> pods = new Dictionary<String, String>();

            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
            IKubernetes client = new Kubernetes(config);
            Console.WriteLine("Starting Request!");

            var list = client.ListNamespacedPod("default");
            var listServices = client.ListNamespacedService("default");


            string ipExternal = "";

            foreach (var service in listServices.Items)
            {
                if (service.Metadata.Name.Contains("azure-minecraft"))
                {
                    ipExternal = service.Status.LoadBalancer.Ingress.First().Ip;
                }

            }

            foreach (var vpod in list.Items)
            {
                if (vpod.Metadata.Name.Contains("azure-minecraft"))
                    pods.Add(vpod.Metadata.Name, ipExternal);
            }


            return pods;

        }


        [HttpPost]
        [Route("//servers")]
        [ValidateModelState]
        [SwaggerOperation("ServersPost")]
        [SwaggerResponse(statusCode: 201, type: typeof(Server), description: "Cria um novo Server")]
        public virtual IActionResult ServersPost(string servername)
        {
            try
            {

                var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
                IKubernetes client = new Kubernetes(config);


                var newPod = client.CreateNamespacedPod(
                    namespaceParameter: "default",

                    body: new V1Pod(
                        metadata: new V1ObjectMeta(name: servername),
                        apiVersion: "apps/v1",

                        kind: "Deployment",

                        spec: new V1PodSpec(

                        containers: new List<V1Container>
                        {
                        new V1Container(
                            image: "openhack/minecraft-server:2.0",
                            name: "azure-minecraft-server",
                            env: new List<V1EnvVar>{new V1EnvVar("EULA","TRUE")},
                            ports:
                                new List<V1ContainerPort> {new V1ContainerPort(containerPort: 25565, name: "client"),
                                new V1ContainerPort(containerPort: 25575, name: "remote") })
                        }
                    ))


                );


                string exampleJson = null;
                exampleJson = "{\n  \"endpoints\" : {\n    \"minecraft\" : \"minecraft\",\n    \"rcon\" : \"rcon\"\n  },\n  \"name\" : \"name\"\n}";


                var example = exampleJson != null
                ? JsonConvert.DeserializeObject<Server>(exampleJson)
                : default(Server);

                example.Name = newPod.Metadata.Name;
                example.Endpoints.Minecraft = newPod.Status.PodIP;

                return new ObjectResult(example);
            }
            catch(Exception ex)
            {
                return new ObjectResult(ex.Message);
            }
        }
    }
}
