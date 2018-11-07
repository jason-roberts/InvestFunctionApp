using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace InvestFunctionApp
{
    public static class BuyStocks
    {
        [FunctionName("BuyStocks")]
        public static async Task Run(
            [QueueTrigger("buy-stocks")]AssetPurchase stockPurchase,
            [Table("Portfolio", InvestorType.Individual)] CloudTable cloudTable,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed for investorId: {stockPurchase.InvestorId}");

            Investor investor = await LoadInvestor(stockPurchase, cloudTable, log);

            AddStocks(stockPurchase, investor);

            await UpdateInvestor(cloudTable, investor);
        }


        private static async Task<Investor> LoadInvestor(AssetPurchase stockPurchase, CloudTable cloudTable, ILogger log)
        {
            TableQuery<Investor> investorQuery =
                new TableQuery<Investor>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, InvestorType.Individual),
                    TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, stockPurchase.InvestorId)));

            var queryExecution = await cloudTable.ExecuteQuerySegmentedAsync(investorQuery, null);

            var investor = queryExecution.Results.FirstOrDefault();

            if (investor is null)
            {
                var message = $"The investor id '{stockPurchase.InvestorId}' was not found in table storage.";
                log.LogError(message);
                throw new ArgumentException(message, nameof(stockPurchase));
            }

            return investor;
        }

        private static void AddStocks(AssetPurchase stocksPurchase, Investor investor)
        {
            investor.CurrentValueOfStocks += stocksPurchase.Amount;
        }

        private static async Task UpdateInvestor(CloudTable cloudTable, Investor investor)
        {
            var operation = TableOperation.Replace(investor);
            await cloudTable.ExecuteAsync(operation);
        }
    }
}
