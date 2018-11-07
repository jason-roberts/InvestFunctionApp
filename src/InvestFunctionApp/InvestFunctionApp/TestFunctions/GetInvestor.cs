using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace InvestFunctionApp.TestFunctions
{
    public static class GetInvestor
    {
        [FunctionName("GetInvestor")]
        public static Investor Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = Testing.TestFunctionRoute + "/getinvestor/{investorId}")]HttpRequest req,
            [Table("Portfolio", InvestorType.Individual, "{investorId}")] Investor investor,            
            ILogger log)
        {
            Testing.AssertInTestEnvironment(log);

            return investor;
        }
    }
}
