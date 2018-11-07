namespace InvestFunctionApp
{
    public interface IInvestementAllocator
    {
        InvestementAllocation Calculate(int totalAmountToBeInvested, Investor currentPortfolio);
    }
}