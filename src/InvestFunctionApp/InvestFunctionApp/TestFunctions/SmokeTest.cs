using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace InvestFunctionApp.TestFunctions
{
    public static class SmokeTest
    {
        [FunctionName("SmokeTest")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = Testing.TestFunctionRoute + "/smoketest")] HttpRequest req,
            ILogger log)
        {            
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            dynamic data = JsonConvert.DeserializeObject(requestBody);

            string azureDevopsReleaseUrl = data?.releaseurl ?? "[releaseurl value not supplied in json request]";

            log.LogInformation($"Smoke test for release {azureDevopsReleaseUrl}");

            // We could add extra smoke testing code here, this simple version just allows us to 
            // verify that the deployment succeeded and the Function App is responding to HTTP requests

            return new OkObjectResult("Smoke test successful");
        }
    }
}
