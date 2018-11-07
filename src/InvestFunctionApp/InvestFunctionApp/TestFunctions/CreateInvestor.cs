using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace InvestFunctionApp.TestFunctions
{
    public static class CreateInvestor
    {
        [FunctionName("CreateInvestor")]
        [return: Table("Portfolio")]
        public static async Task<Investor> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = Testing.TestFunctionRoute + "/createinvestor")] HttpRequest req,            
            ILogger log)
        {
            Testing.AssertInTestEnvironment(log);

            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            return JsonConvert.DeserializeObject<Investor>(requestBody);
        }
    }
}
