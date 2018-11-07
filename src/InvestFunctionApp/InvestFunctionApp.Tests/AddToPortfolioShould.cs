using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace InvestFunctionApp.Tests
{
    public class AddToPortfolioShould
    {
        [Fact]
        public async Task ReturnCorrectDepositInformation()
        {
            var deposit = new Deposit { Amount = 42 };
            var investor = new Investor { };

            Mock<HttpRequest> mockRequest = CreateMockRequest(deposit);

            DepositRequest result = await Portfolio.Run(mockRequest.Object, investor, "42", new Mock<ILogger>().Object);

            Assert.Equal(42, result.Amount);
            Assert.Same(investor, result.Investor);
        }

        [Fact]
        public async Task ErrorWhenInvestorDoesNotExist()
        {
            var deposit = new Deposit { Amount = 42 };

            Mock<HttpRequest> mockRequest = CreateMockRequest(deposit);

            await Assert.ThrowsAsync<ArgumentException>(() => Portfolio.Run(mockRequest.Object, null, "42", new Mock<ILogger>().Object));
        }

        private static Mock<HttpRequest> CreateMockRequest(object body)
        {            
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            var json = JsonConvert.SerializeObject(body);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);

            return mockRequest;
        }
    }
}