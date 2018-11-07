using Microsoft.WindowsAzure.Storage.Table;

namespace InvestFunctionApp
{
    public class Investor : TableEntity
    {
        public string Name { get; set; }

        // No built-in support for decimals with table storage so
        // use int to keep demo simple and not worry about customizing
        // EntityResolver or overriding ReadEntity and WriteEntity, etc.
        public int CurrentValueOfStocks { get; set; } 
        public int CurrentValueOfBonds { get; set; } 

        public int TargetPercentageAllocationToStocks { get; set; }
        public int TargetPercentageAllocationToBonds { get; set; }

        public override string ToString()
        {
            return $"Name: {Name} - Current values (stocks/bonds) {CurrentValueOfStocks} {CurrentValueOfBonds} - Targets (stocks/bonds) {TargetPercentageAllocationToStocks} {TargetPercentageAllocationToBonds}";
        }
    }    
}
