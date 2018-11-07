using System;
using Xunit;

namespace InvestFunctionApp.Tests
{
    public class NaiveInvestementAllocatorShould
    {
        [Fact]
        public void AllocateAllToBondsWhenUnderweight()
        {
            var investor = new Investor
            {
                CurrentValueOfBonds = 1,
                CurrentValueOfStocks = 2,
                TargetPercentageAllocationToBonds = 50,
                TargetPercentageAllocationToStocks = 50
            };

            var sut = new NaiveInvestementAllocator();

            var r = sut.Calculate(100, investor);

            Assert.Equal(100, r.AmountToBonds);
            Assert.Equal(0, r.AmountToStocks);
        }

        [Fact]
        public void AllocateAllToStocksWhenUnderweight()
        {
            var investor = new Investor
            {
                CurrentValueOfBonds = 2,
                CurrentValueOfStocks = 1,
                TargetPercentageAllocationToBonds = 50,
                TargetPercentageAllocationToStocks = 50
            };

            var sut = new NaiveInvestementAllocator();

            var r = sut.Calculate(100, investor);
            
            Assert.Equal(100, r.AmountToStocks);
            Assert.Equal(0, r.AmountToBonds);
        }

        [Fact]
        public void AllocateAllToBondsWhenPerfectlyBalanced()
        {
            var investor = new Investor
            {
                CurrentValueOfBonds = 1,
                CurrentValueOfStocks = 1,
                TargetPercentageAllocationToBonds = 50,
                TargetPercentageAllocationToStocks = 50
            };

            var sut = new NaiveInvestementAllocator();

            var r = sut.Calculate(100, investor);

            Assert.Equal(0, r.AmountToStocks);
            Assert.Equal(100, r.AmountToBonds);
        }

        [Fact]
        public void ErrorWhenZeroAmountToBeInvested()
        {
            var investor = new Investor
            {
                CurrentValueOfBonds = 1,
                CurrentValueOfStocks = 1,
                TargetPercentageAllocationToBonds = 50,
                TargetPercentageAllocationToStocks = 50
            };

            var sut = new NaiveInvestementAllocator();

            Assert.Throws<ArgumentOutOfRangeException>(() => sut.Calculate(0, investor));
        }
    }
}
