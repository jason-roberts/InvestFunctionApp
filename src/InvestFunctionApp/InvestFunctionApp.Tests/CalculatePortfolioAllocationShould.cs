using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace InvestFunctionApp.Tests
{
    public class CalculatePortfolioAllocationShould
    {
        [Fact]
        public void WriteToPurchaseBondQueue()
        {
            const int amount = 42;

            var deposit = new DepositRequest { Amount = amount, Investor = new Investor { } };

            var mockAllocator = new Mock<IInvestementAllocator>();
            mockAllocator.Setup(x => x.Calculate(amount, deposit.Investor))
                         .Returns(new InvestementAllocation { AmountToBonds = 1, AmountToStocks = 0 });

            CalculatePortfolioAllocation.Run(deposit, out var stockPurchase, out var bondPurchase,mockAllocator.Object, new Mock<ILogger>().Object);

            Assert.Equal(1, bondPurchase.Amount);
            Assert.Null(stockPurchase);
        }

        [Fact]
        public void WriteToPurchaseStockQueue()
        {
            const int amount = 42;

            var deposit = new DepositRequest { Amount = amount, Investor = new Investor { } };

            var mockAllocator = new Mock<IInvestementAllocator>();
            mockAllocator.Setup(x => x.Calculate(amount, deposit.Investor))
                         .Returns(new InvestementAllocation { AmountToBonds = 0, AmountToStocks = 1 });

            CalculatePortfolioAllocation.Run(deposit, out var stockPurchase, out var bondPurchase, mockAllocator.Object, new Mock<ILogger>().Object);

            Assert.Equal(1, stockPurchase.Amount);
            Assert.Null(bondPurchase);
        }

        [Fact]
        public void WriteToPurchaseStockAndBondsQueue()
        {
            const int amount = 42;

            var deposit = new DepositRequest { Amount = amount, Investor = new Investor { } };

            var mockAllocator = new Mock<IInvestementAllocator>();
            mockAllocator.Setup(x => x.Calculate(amount, deposit.Investor))
                         .Returns(new InvestementAllocation { AmountToBonds = 2, AmountToStocks = 1 });

            CalculatePortfolioAllocation.Run(deposit, out var stockPurchase, out var bondPurchase, mockAllocator.Object, new Mock<ILogger>().Object);

            Assert.Equal(1, stockPurchase.Amount);
            Assert.Equal(2, bondPurchase.Amount);
        }

        [Fact]
        public void ErrorIfNoAllocationsToStocksOrBonds()
        {
            const int amount = 42;

            var deposit = new DepositRequest { Amount = amount, Investor = new Investor { } };

            var mockAllocator = new Mock<IInvestementAllocator>();
            mockAllocator.Setup(x => x.Calculate(amount, deposit.Investor))
                         .Returns(new InvestementAllocation { AmountToBonds = 0, AmountToStocks = 0 });

            Assert.Throws<Exception>(() => 
            CalculatePortfolioAllocation.Run(deposit, out var stockPurchase, out var bondPurchase, mockAllocator.Object, new Mock<ILogger>().Object));
         }
    }
}