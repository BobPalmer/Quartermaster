using Xunit;

namespace WOLF.Tests.Unit
{
    public class When_exploring_contracts
    {
        [Fact]
        public void Contracts_should_be_active_by_default()
        {
            var contract = new Contract();
            var expectedState = ContractState.Active;

            Assert.Equal(expectedState, contract.State);
        }

        [Fact]
        public void Contracts_should_default_to_per_day_rate()
        {
            var contract = new Contract();
            var expectedRate = ContractRateUnit.PerDay;

            Assert.Equal(expectedRate, contract.Rate);
        }

        [Fact]
        public void Contracts_should_not_expire_by_default()
        {
            var contract = new Contract();
            var expectedExpiration = 0d;

            Assert.Equal(expectedExpiration, contract.ExpirationTimestamp);
        }

        [Fact]
        public void Contracts_should_have_zero_priority_by_default()
        {
            var contract = new Contract();
            var expectedPriority = 0;

            Assert.Equal(expectedPriority, contract.Priority);
        }
    }
}
