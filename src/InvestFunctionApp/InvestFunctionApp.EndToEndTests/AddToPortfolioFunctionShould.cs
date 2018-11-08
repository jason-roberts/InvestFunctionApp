using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace InvestFunctionApp.EndToEndTests
{
    public class AddToPortfolioFunctionShould
    {
        private string CreateInvestorFunctionKey;
        private string GetInvestorFunctionKey;
        private string PortfolioFunctionKey;
        private readonly ITestOutputHelper Output;

        public AddToPortfolioFunctionShould(ITestOutputHelper output)
        {
            Output = output;
            ReadTestEnvironmentVariables();            
        }

        [Fact]
        public async Task BuyStocks()
        {
            const int initialValueOfStocks = 100;
            const int amountToInvest = 42;

            var startingInvestorDetails = new Investor
            {
                PartitionKey = "IndividualInvestor",
                RowKey = Guid.NewGuid().ToString(),
                CurrentValueOfStocks = initialValueOfStocks,
                CurrentValueOfBonds = 100,
                TargetPercentageAllocationToStocks = 75,
                TargetPercentageAllocationToBonds = 25
            };
            
            await CreateTestInvestorInTableStorage(startingInvestorDetails);
            await InvokeAddToPortfolioFunction(startingInvestorDetails.RowKey, amountToInvest);

            // Wait for a while
            await Task.Delay(TimeSpan.FromMinutes(2));

            var resultingInvestor = await GetInvestor(startingInvestorDetails.RowKey);

            Assert.Equal(initialValueOfStocks + amountToInvest, resultingInvestor.CurrentValueOfStocks);
        }



        private async Task CreateTestInvestorInTableStorage(Investor investor)
        {
            HttpClient client = new HttpClient();            
            HttpResponseMessage response = await client.PostAsJsonAsync($"https://investfunctionappdemotest.azurewebsites.net/api/testing/createinvestor?code={CreateInvestorFunctionKey}", investor);
            response.EnsureSuccessStatusCode();
        }


        private async Task InvokeAddToPortfolioFunction(string investorId, int amount)
        {
            var url = $"https://investfunctionappdemotest.azurewebsites.net/api/portfolio/{investorId}?code={PortfolioFunctionKey}";

            var deposit = new Deposit { Amount = amount };

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsJsonAsync(url, deposit);
            response.EnsureSuccessStatusCode();
        }

        private async Task<Investor> GetInvestor(string investorId)
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync($"https://investfunctionappdemotest.azurewebsites.net/api/testing/getinvestor/{investorId}?code={GetInvestorFunctionKey}");
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<Investor>(await response.Content.ReadAsStringAsync());
        }

        private void ReadTestEnvironmentVariables()
        {
            CreateInvestorFunctionKey = ReadEnvironmentVariable("CreateInvestorFunctionKey");
            GetInvestorFunctionKey = ReadEnvironmentVariable("GetInvestorInvestorFunctionKey");
            PortfolioFunctionKey = ReadEnvironmentVariable("PortfolioFunctionKey");
        }

        private string ReadEnvironmentVariable(string variableName)
        {
            var value = Environment.GetEnvironmentVariable(variableName);
            Output.WriteLine($"'{variableName}'='{value ?? "NOT SET"}'");
            return value;
        }
    }
}
