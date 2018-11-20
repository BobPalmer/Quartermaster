using System;
using WOLF.Tests.Unit.Mocks;
using Xunit;

namespace WOLF.Tests.Unit
{
    public class When_exploring_endpoints
    {
        [Fact]
        public void An_endpoint_should_not_have_an_id_by_default()
        {
            var endpoint = new MockEndpoint();

            Assert.Null(endpoint.Id);
        }

        [Fact]
        public void An_endpoint_should_have_no_contracts_by_default()
        {
            var endpoint = new MockEndpoint();

            Assert.NotNull(endpoint.Contracts);
            Assert.Empty(endpoint.Contracts);
        }

        [Fact]
        public void An_endpoint_should_have_zero_incoming_by_default()
        {
            var endpoint = new MockEndpoint();
            var expectedQuantity = 0d;

            var actualQuantity = endpoint.Incoming("Ore", ContractRateUnit.PerDay);

            Assert.Equal(expectedQuantity, actualQuantity);
        }

        [Fact]
        public void An_endpoint_should_have_zero_outgoing_by_default()
        {
            var endpoint = new MockEndpoint();
            var expectedQuantity = 0d;

            var actualQuantity = endpoint.Outgoing("Ore", ContractRateUnit.PerDay);

            Assert.Equal(expectedQuantity, actualQuantity);
        }

        [Fact]
        public void An_endpoint_should_have_zero_free_capacity_by_default()
        {
            var endpoint = new MockEndpoint();
            var expectedQuantity = 0d;

            var actualQuantity = endpoint.Free("Ore", ContractRateUnit.PerDay);

            Assert.Equal(expectedQuantity, actualQuantity);
        }

        [Fact]
        public void Incoming_contracts_should_increase_free_capacity()
        {
            var endpointA = new MockEndpoint();
            var endpointB = new MockEndpoint();

            var expectedResourceName = "Ore";
            var expectedFreeQuantity = 50d;
            var expectedContractRate = ContractRateUnit.PerDay;

            var contract = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpointA,
                Destination = endpointB,
                ResourceName = expectedResourceName,
                Quantity = expectedFreeQuantity,
                Rate = expectedContractRate
            };

            endpointB.Contracts.Add(contract);

            var canFulfill = endpointB.CanProvide(expectedResourceName, expectedFreeQuantity, expectedContractRate);
            var actualFreeQuantity = endpointB.Free(expectedResourceName, expectedContractRate);

            Assert.True(canFulfill);
            Assert.Equal(expectedFreeQuantity, actualFreeQuantity);
        }

        [Fact]
        public void Outgoing_contracts_should_decrease_free_capacity()
        {
            var endpointA = new MockEndpoint();
            var endpointB = new MockEndpoint();
            var endpointC = new MockEndpoint();

            var resourceName = "Ore";
            var contractRate = ContractRateUnit.PerDay;

            var contractA = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpointA,
                Destination = endpointB,
                ResourceName = resourceName,
                Quantity = 50d,
                Rate = contractRate
            };
            var contractB = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpointB,
                Destination = endpointC,
                ResourceName = resourceName,
                Quantity = 25d,
                Rate = contractRate
            };

            endpointB.Contracts.Add(contractA);
            endpointB.Contracts.Add(contractB);

            var expectedFreeQuantity = 25d;
            var actualFreeQuantity = endpointB.Free(resourceName, contractRate);

            Assert.Equal(expectedFreeQuantity, actualFreeQuantity);
        }

        [Fact]
        public void An_endpoint_should_be_able_to_convert_between_quantity_rates()
        {
            var endpointA = new MockEndpoint();
            var endpointB = new MockEndpoint();

            var resourceName = "Ore";
            var inputQuantity = 50d;
            var inputRate = ContractRateUnit.PerHour;

            var contract = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpointA,
                Destination = endpointB,
                ResourceName = resourceName,
                Quantity = inputQuantity,
                Rate = inputRate
            };

            endpointB.Contracts.Add(contract);

            var convertToRate = ContractRateUnit.PerDay;
            var expectedFreeQuantity = inputQuantity * 6d;

            var canFulfill = endpointB.CanProvide(resourceName, expectedFreeQuantity, convertToRate);
            var actualFreeQuantity = endpointB.Free(resourceName, convertToRate);

            Assert.True(canFulfill);
            Assert.Equal(expectedFreeQuantity, actualFreeQuantity);
        }

        [Fact]
        public void Attached_contracts_should_be_broken_when_an_endpoint_is_destroyed()
        {
            var expectedState = ContractState.Disposed;
            var endpointA = new MockEndpoint();
            var endpointB = new MockEndpoint();
            var contractA = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpointA,
                Destination = endpointB
            };
            var contractB = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpointB,
                Destination = endpointA
            };

            endpointB.Contracts.Add(contractA);
            endpointB.Contracts.Add(contractB);

            endpointB.Dispose();

            Assert.Equal(expectedState, contractA.State);
            Assert.Equal(expectedState, contractB.State);
        }

        [Theory]
        [InlineData(100d, 50d, ContractState.Active, true)]
        [InlineData(50d, 50d, ContractState.Active, true)]
        [InlineData(50d, 100d, ContractState.Active, false)]
        [InlineData(100d, 50d, ContractState.Broken, false)]
        [InlineData(100d, 50d, ContractState.Expired, false)]
        [InlineData(100d, 50d, ContractState.Inactive, false)]
        public void An_endpoint_should_know_if_it_can_honor_its_contracts(double incomingQuantity, double outgoingQuantity, ContractState incomingState, bool expectedResult)
        {
            var endpointA = new MockEndpoint();
            var endpointB = new MockEndpoint();
            var endpointC = new MockEndpoint();
            var resourceName = "Ore";
            var contractA = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpointA,
                Destination = endpointB,
                ResourceName = resourceName,
                Quantity = incomingQuantity,
                State = incomingState
            };
            var contractB = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpointB,
                Destination = endpointC,
                ResourceName = resourceName,
                Quantity = outgoingQuantity
            };

            endpointB.Contracts.Add(contractA);
            endpointB.Contracts.Add(contractB);

            var actualResult = endpointB.IsHonoringContracts(resourceName);

            Assert.Equal(expectedResult, actualResult);
        }
    }
}
