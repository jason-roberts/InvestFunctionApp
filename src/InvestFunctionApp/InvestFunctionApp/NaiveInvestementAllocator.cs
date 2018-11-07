using System;

namespace InvestFunctionApp
{
    // Simply allocates total amount to underweight asset class rather than calculating "spread"
    public class NaiveInvestementAllocator : IInvestementAllocator
    {
        public InvestementAllocation Calculate(int totalAmountToBeInvested, Investor investor)
        {
            if (totalAmountToBeInvested <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalAmountToBeInvested), "The amount must be greater than zero.");
            }

            var totalCurrentPortfolioValue = investor.CurrentValueOfBonds + investor.CurrentValueOfStocks;

            decimal currentPercentageBonds = CalculateCurrentBondsPercentage(investor, totalCurrentPortfolioValue);

            bool areBondsOverweight = currentPercentageBonds > investor.TargetPercentageAllocationToBonds;

            if (areBondsOverweight)
            {
                return AllocateAllToStocks(totalAmountToBeInvested);
            }

            return AllocateAllToBonds(totalAmountToBeInvested);

        }

        private static decimal CalculateCurrentBondsPercentage(Investor investor, decimal totalCurrentPortfolioValue)
        {            
            // Normally all financial amounts would be type decimal, ints are used in this sample to simplify table storage
            return ((decimal)investor.CurrentValueOfBonds / totalCurrentPortfolioValue) * 100;
        }

        private InvestementAllocation AllocateAllToStocks(int amount)
        {
            return new InvestementAllocation
            {
                AmountToBonds = 0,
                AmountToStocks = amount
            };
        }

        private InvestementAllocation AllocateAllToBonds(int amount)
        {
            return new InvestementAllocation
            {
                AmountToBonds = amount,
                AmountToStocks = 0
            };
        }
    }
}
