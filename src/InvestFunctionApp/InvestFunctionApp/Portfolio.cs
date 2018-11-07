using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace InvestFunctionApp
{
    public static class Portfolio
    {
        [FunctionName("Portfolio")]
        [return: Queue("deposit-requests")]
        public static async Task<DepositRequest> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "portfolio/{investorId}")]HttpRequest req,
            [Table("Portfolio", InvestorType.Individual, "{investorId}")] Investor investor,
            string investorId,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            log.LogInformation($"Request body: {requestBody}");

            var deposit = JsonConvert.DeserializeObject<Deposit>(requestBody);

            if (investor == null)
            {
                throw new ArgumentException($"Invalid investorId '{investorId}.");
            }
            if (deposit is null)
            {
                throw new ArgumentException($"Invalid deposit.");
            }
            if (deposit.Amount <= 0)
            {
                throw new ArgumentException($"Deposit amount must be greater than 1.");
            }

            // Additional validation omitted for demo purposes

            var depositRequest = new DepositRequest
            {
                Amount = deposit.Amount,
                Investor = investor
            };

            log.LogInformation($"Deposit created: {depositRequest}");

            return depositRequest;
        }
    }
}
