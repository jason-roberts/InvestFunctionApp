using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace InvestFunctionApp
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class CalculatePortfolioAllocation
    {
        [FunctionName("CalculatePortfolioAllocation")]
        public static void Run(
            [QueueTrigger("deposit-requests")]DepositRequest depositRequest,
            [Queue("buy-stocks")] out AssetPurchase stockPurchase,
            [Queue("buy-bonds")] out AssetPurchase bondPurchase,
            [Inject] IInvestementAllocator investementAllocator,
            ILogger log)
        {
            stockPurchase = null;
            bondPurchase = null;

            log.LogInformation($"C# Queue trigger function processed: {depositRequest}");

            InvestementAllocation allocation = investementAllocator.Calculate(depositRequest.Amount, depositRequest.Investor);

            if (allocation.AmountToBonds > 0)
            {
                log.LogInformation($"Allocating {allocation.AmountToBonds} to bonds");
                bondPurchase = new AssetPurchase { InvestorId = depositRequest.Investor.RowKey, Amount = allocation.AmountToBonds };
            }
            if (allocation.AmountToStocks > 0)
            {
                log.LogInformation($"Allocating {allocation.AmountToStocks} to stocks");
                stockPurchase = new AssetPurchase { InvestorId = depositRequest.Investor.RowKey, Amount = allocation.AmountToStocks };
            }

            if (bondPurchase is null && stockPurchase is null)
            {
                throw new System.Exception($"The deposit request for {depositRequest.Amount} did not result in an allocation to stocks or bonds. Check the input amount and the correctness of the IInvestementAllocator being used.");
            }
        }
    }
}
