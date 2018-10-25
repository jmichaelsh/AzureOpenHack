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
//using KubeClient;
//using KubeClient.Models;
//using KubeClient.Extensions;
using k8s;
using k8s.Models;


namespace Backend_API_Minecraft.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {

        [HttpDelete]
        [Route("//servers")]
        [ValidateModelState]
        [SwaggerOperation("ServersDelete")]
        public virtual IActionResult ServersDelete([FromRoute][Required]string name)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);


            throw new NotImplementedException();
        }


        [HttpGet]
        [Route("//servers")]
        [ValidateModelState]
        [SwaggerOperation("ServersGet")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<Server>), description: "retorna lista de servers")]
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
        public virtual IActionResult ServersPost([FromBody]string name)
        {

            throw new NotImplementedException();
        }
    }
}
