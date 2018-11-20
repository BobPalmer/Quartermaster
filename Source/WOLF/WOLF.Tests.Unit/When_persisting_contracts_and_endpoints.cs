using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOLF.Tests.Unit.Mocks;
using Xunit;

namespace WOLF.Tests.Unit
{
    public class When_persisting_contracts_and_endpoints
    {
        [Fact]
        public void Contracts_should_be_persisted_on_save()
        {
            var endpointA = new MockEndpoint { Id = Guid.NewGuid().ToString() };
            var endpointB = new MockEndpoint { Id = Guid.NewGuid().ToString() };
            var contract1 = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpointA,
                Destination = endpointB,
                ResourceName = "Ore",
                Quantity = 100d,
                Rate = ContractRateUnit.PerMinute,
                ExpirationTimestamp = 1000d,
                Priority = 10
            };
            var contract2 = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = endpointA,
                Destination = endpointB,
                ResourceName = "ElectricCharge",
                Quantity = 10d,
                Rate = ContractRateUnit.PerSecond
            };
            var negotiator = new MockNegotiator();
            negotiator.Contracts.Add(contract1);
            negotiator.Contracts.Add(contract2);
            negotiator.Endpoints.Add(endpointA);
            negotiator.Endpoints.Add(endpointB);

            var persister = new ScenarioPersister();
            var configNode = new ConfigNode();

            persister.OnSave(configNode, negotiator);

            var expectedNodeName = "WOLF_DATA";
            Assert.True(configNode.HasNode(expectedNodeName));
        }

        [Fact(Skip = "Not yet implemented")]
        public void Endpoints_should_be_persisted_on_save()
        {

        }
    }
}
