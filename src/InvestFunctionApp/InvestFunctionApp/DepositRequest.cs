namespace InvestFunctionApp
{
    public class DepositRequest
    {
        public Investor Investor { get; set; }
        public int Amount { get; set; }

        public override string ToString()
        {
            return base.ToString() + $" Amount: {Amount}"; 
        }
    }
}
