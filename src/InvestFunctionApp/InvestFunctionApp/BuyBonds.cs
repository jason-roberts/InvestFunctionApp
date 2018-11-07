using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace InvestFunctionApp
{
    public static class BuyBonds
    {
        [FunctionName("BuyBonds")]
        public static async Task Run(
            [QueueTrigger("buy-bonds")]AssetPurchase bondPurchase,
            [Table("Portfolio", InvestorType.Individual)] CloudTable cloudTable,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed for investorId: {bondPurchase.InvestorId}");

            Investor investor = await LoadInvestor(bondPurchase, cloudTable, log);

            AddBonds(bondPurchase, investor);

            await UpdateInvestor(cloudTable, investor);
        }

        private static async Task<Investor> LoadInvestor(AssetPurchase bondPurchase, CloudTable cloudTable, ILogger log)
        {
            TableQuery<Investor> investorQuery =
                new TableQuery<Investor>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, InvestorType.Individual),
                    TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, bondPurchase.InvestorId)));

            var queryExecution = await cloudTable.ExecuteQuerySegmentedAsync(investorQuery, null);

            var investor = queryExecution.Results.FirstOrDefault();

            if (investor is null)
            {
                var message = $"The investor id '{bondPurchase.InvestorId}' was not found in table storage.";
                log.LogError(message);
                throw new ArgumentException(message, nameof(bondPurchase));
            }

            return investor;
        }

        private static void AddBonds(AssetPurchase bondPurchase, Investor investor)
        {
            investor.CurrentValueOfBonds += bondPurchase.Amount;
        }

        private static async Task UpdateInvestor(CloudTable cloudTable, Investor investor)
        {
            var operation = TableOperation.Replace(investor);
            await cloudTable.ExecuteAsync(operation);
        }
    }
}
