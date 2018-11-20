using System;
using WOLF.Tests.Unit.Mocks;
using Xunit;

namespace WOLF.Tests.Unit
{
    public class When_exploring_the_contractnegotiator
    {
        [Fact]
        public void An_endpoint_can_be_registered()
        {
            var negotiator = new ContractNegotiator();
            var expectedType = "MockEndpoint";
            var endpoint = new MockEndpoint();

            var endpointId = negotiator.AddEndpoint(endpoint);

            Assert.Equal(endpointId, endpoint.Id);
            Assert.Equal(expectedType, endpoint.Type);
        }

        [Fact]
        public void An_endpoint_can_be_removed()
        {
            var negotiator = new ContractNegotiator();
            var endpoint = new MockEndpoint();

            var endpointId = negotiator.AddEndpoint(endpoint);
            negotiator.RemoveEndpoint(endpointId);

            var findResult = negotiator.FindEndpoint(endpointId);

            Assert.Null(findResult);
        }

        [Fact]
        public void Endpoint_ids_should_be_unique()
        {
            var negotiator = new ContractNegotiator();
            var endpointA = new MockEndpoint();
            var endpointB = new MockEndpoint();

            var endpointIdA = negotiator.AddEndpoint(endpointA);
            var endpointIdB = negotiator.AddEndpoint(endpointB);

            Assert.NotEqual(endpointIdA, endpointIdB);
            Assert.NotEqual(endpointA.Id, endpointB.Id);
        }

        [Fact]
        public void An_endpoint_can_be_located_by_id()
        {
            var negotiator = new ContractNegotiator();
            var expectedEndpoint = new MockEndpoint();
            var expectedEndpointId = negotiator.AddEndpoint(expectedEndpoint);

            var actualEndpoint = negotiator.FindEndpoint(expectedEndpointId);

            Assert.NotNull(actualEndpoint);
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }

        [Fact]
        public void A_contract_can_be_negotiated_between_endpoints()
        {
            var negotiator = new ContractNegotiator();
            var endpoint0 = new MockEndpoint();
            var endpointA = new MockEndpoint();
            var endpointB = new MockEndpoint();

            var endpointId0 = negotiator.AddEndpoint(endpoint0);
            var endpointIdA = negotiator.AddEndpoint(endpointA);
            var endpointIdB = negotiator.AddEndpoint(endpointB);

            var resourceName = "ResourceA";
            var requestedQuantity = 25d;
            var rate = ContractRateUnit.PerDay;

            var contract0 = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpoint0,
                Destination = endpointA,
                ResourceName = resourceName,
                Quantity = 50d,
                Rate = rate
            };

            endpoint0.Contracts.Add(contract0);
            endpointA.Contracts.Add(contract0);

            var result = negotiator.Negotiate(endpointIdA, endpointIdB, resourceName, requestedQuantity, rate);

            var okResult = Assert.IsType<OkNegotiationResult<Contract>>(result);
            var contract = okResult.Contract;

            Assert.Equal(endpointA, contract.Source);
            Assert.Equal(endpointB, contract.Destination);
            Assert.Equal(resourceName, contract.ResourceName);
            Assert.Equal(requestedQuantity, contract.Quantity);
            Assert.Equal(rate, contract.Rate);
        }

        [Fact]
        public void Contract_ids_should_be_unique()
        {
            var negotiator = new ContractNegotiator();
            var endpoint0 = new MockEndpoint();
            var endpointA = new MockEndpoint();
            var endpointB = new MockEndpoint();

            var endpointId0 = negotiator.AddEndpoint(endpoint0);
            var endpointIdA = negotiator.AddEndpoint(endpointA);
            var endpointIdB = negotiator.AddEndpoint(endpointB);

            var resourceName = "ResourceB";
            var requestedQuantity = 25d;
            var rate = ContractRateUnit.PerDay;

            var contract0 = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpoint0,
                Destination = endpointA,
                ResourceName = resourceName,
                Quantity = 100d,
                Rate = rate
            };

            endpoint0.Contracts.Add(contract0);
            endpointA.Contracts.Add(contract0);

            var resultA = negotiator.Negotiate(endpointIdA, endpointIdB, resourceName, requestedQuantity, rate);
            var resultB = negotiator.Negotiate(endpointIdA, endpointIdB, resourceName, requestedQuantity, rate);

            var okResultA = Assert.IsType<OkNegotiationResult<Contract>>(resultA);
            var contractA = okResultA.Contract;
            var okResultB = Assert.IsType<OkNegotiationResult<Contract>>(resultB);
            var contractB = okResultB.Contract;

            Assert.NotEqual(contractA.ContractId, contractB.ContractId);
        }

        [Fact]
        public void A_contract_can_be_located_by_id()
        {
            var negotiator = new ContractNegotiator();
            var endpoint0 = new MockEndpoint();
            var endpointA = new MockEndpoint();
            var endpointB = new MockEndpoint();

            var endpointId0 = negotiator.AddEndpoint(endpoint0);
            var endpointIdA = negotiator.AddEndpoint(endpointA);
            var endpointIdB = negotiator.AddEndpoint(endpointB);

            var resourceName = "ResourceC";
            var requestedQuantity = 25d;
            var rate = ContractRateUnit.PerDay;

            var contract0 = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpoint0,
                Destination = endpointA,
                ResourceName = resourceName,
                Quantity = 50d,
                Rate = rate
            };

            endpoint0.Contracts.Add(contract0);
            endpointA.Contracts.Add(contract0);
            var negotiationResult = negotiator.Negotiate(endpointIdA, endpointIdB, resourceName, requestedQuantity, rate);
            var okResult = Assert.IsType<OkNegotiationResult<Contract>>(negotiationResult);
            var expectedContract = okResult.Contract;

            var actualContract = negotiator.FindContract(expectedContract.ContractId);

            Assert.NotNull(actualContract);
            Assert.Equal(expectedContract, actualContract);
        }

        [Theory]
        [InlineData(ContractState.Broken)]
        [InlineData(ContractState.Disposed)]
        [InlineData(ContractState.Expired)]
        [InlineData(ContractState.Inactive)]
        public void Failed_contracts_should_cascade_to_dependent_contracts(ContractState failureState)
        {
            var negotiator = new ContractNegotiator();
            var endpoint0 = new MockEndpoint();
            var endpointA = new MockEndpoint();
            var endpointB = new MockEndpoint();
            var endpointC = new MockEndpoint();
            var endpointD = new MockEndpoint();

            var endpointIdA = negotiator.AddEndpoint(endpointA);
            var endpointIdB = negotiator.AddEndpoint(endpointB);
            var endpointIdC = negotiator.AddEndpoint(endpointC);
            var endpointIdD = negotiator.AddEndpoint(endpointD);

            var resourceName = "Ore";
            var contract0 = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpoint0,
                Destination = endpointA,
                ResourceName = resourceName,
                Quantity = 100d
            };
            endpointA.Contracts.Add(contract0);

            var negotiationA = negotiator.Negotiate(endpointIdA, endpointIdB, resourceName, 100d) as OkNegotiationResult<Contract>;
            var negotiationB = negotiator.Negotiate(endpointIdB, endpointIdC, resourceName, 50d) as OkNegotiationResult<Contract>;
            var negotiationC = negotiator.Negotiate(endpointIdC, endpointIdD, resourceName, 25d) as OkNegotiationResult<Contract>;

            var contractAB = negotiationA.Contract;
            var contractBC = negotiationB.Contract;
            var contractCD = negotiationC.Contract;

            contractAB.State = failureState;
            negotiator.ValidateContracts();

            var expectedResult = false;
            var actualResult = endpointC.IsHonoringContracts(resourceName);

            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void Contract_priorities_should_be_enforced()
        {
            var negotiator = new ContractNegotiator();
            var endpoint0 = new MockEndpoint();
            var endpointA = new MockEndpoint();
            var endpointB = new MockEndpoint();
            var endpointC = new MockEndpoint();

            var endpointIdA = negotiator.AddEndpoint(endpointA);
            var endpointIdB = negotiator.AddEndpoint(endpointB);
            var endpointIdC = negotiator.AddEndpoint(endpointC);

            var resourceName = "Ore";
            var contract0 = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpoint0,
                Destination = endpointA,
                ResourceName = resourceName,
                Quantity = 100d
            };
            endpointA.Contracts.Add(contract0);

            // Negotiate starting input and output contracts
            var negotiationA1 = negotiator.Negotiate(endpointIdA, endpointIdB, resourceName, 50d) as OkNegotiationResult<Contract>;
            var negotiationA2 = negotiator.Negotiate(endpointIdA, endpointIdB, resourceName, 50d) as OkNegotiationResult<Contract>;
            var negotiationB1 = negotiator.Negotiate(endpointIdB, endpointIdC, resourceName, 50d) as OkNegotiationResult<Contract>;
            var negotiationB2 = negotiator.Negotiate(endpointIdB, endpointIdC, resourceName, 25d) as OkNegotiationResult<Contract>;

            var contractA1 = negotiationA1.Contract;
            var contractA2 = negotiationA2.Contract;
            var contractB1 = negotiationB1.Contract;
            var contractB2 = negotiationB2.Contract;

            // Setup contract priorities (larger number == higher priority)
            contractB1.Priority = 1;
            contractB2.Priority = 10;

            // Break an input contract
            contractA1.State = ContractState.Broken;
            negotiator.ValidateContracts();

            var expectedB1State = ContractState.Broken;
            var expectedB2State = ContractState.Active;

            Assert.Equal(expectedB1State, contractB1.State);
            Assert.Equal(expectedB2State, contractB2.State);
        }

        [Fact]
        public void Failed_contracts_should_be_reactivated_when_requirements_are_restored()
        {
            var negotiator = new ContractNegotiator();
            var endpoint0 = new MockEndpoint();
            var endpointA = new MockEndpoint();
            var endpointB = new MockEndpoint();
            var endpointC = new MockEndpoint();

            var endpointIdA = negotiator.AddEndpoint(endpointA);
            var endpointIdB = negotiator.AddEndpoint(endpointB);
            var endpointIdC = negotiator.AddEndpoint(endpointC);

            var resourceName = "Ore";
            var contract0 = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpoint0,
                Destination = endpointA,
                ResourceName = resourceName,
                Quantity = 200d
            };
            endpointA.Contracts.Add(contract0);

            // Negotiate starting input and output contracts
            var negotiationA1 = negotiator.Negotiate(endpointIdA, endpointIdB, resourceName, 50d) as OkNegotiationResult<Contract>;
            var negotiationA2 = negotiator.Negotiate(endpointIdA, endpointIdB, resourceName, 50d) as OkNegotiationResult<Contract>;
            var negotiationB1 = negotiator.Negotiate(endpointIdB, endpointIdC, resourceName, 50d) as OkNegotiationResult<Contract>;
            var negotiationB2 = negotiator.Negotiate(endpointIdB, endpointIdC, resourceName, 25d) as OkNegotiationResult<Contract>;

            var contractA1 = negotiationA1.Contract;
            var contractA2 = negotiationA2.Contract;
            var contractB1 = negotiationB1.Contract;
            var contractB2 = negotiationB2.Contract;

            // Break an input contract
            contractA1.State = ContractState.Broken;
            negotiator.ValidateContracts();

            // Negotiate an additional input contract
            var negotiationA3 = negotiator.Negotiate(endpointIdA, endpointIdB, resourceName, 25d) as OkNegotiationResult<Contract>;

            var expectedB1State = ContractState.Active;
            var expectedB2State = ContractState.Active;

            Assert.Equal(expectedB1State, contractB1.State);
            Assert.Equal(expectedB2State, contractB2.State);
        }
    }
}
