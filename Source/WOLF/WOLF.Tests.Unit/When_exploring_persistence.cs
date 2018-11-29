using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOLF.Tests.Unit.Mocks;
using Xunit;

namespace WOLF.Tests.Unit
{
    public class When_exploring_persistence
    {
        [Fact]
        public void Depots_should_be_persisted()
        {
            var configNode = new ConfigNode();
            var expectedBody = "Mun";
            var expectedBiome = "East Crater";
            var expectedSituation = Vessel.Situations.LANDED;
            var depot = new Depot(expectedBody, expectedBiome, expectedSituation);
            var persister = new ScenarioPersister();
            persister.Depots.Add(depot);

            var expectedResource = "ElectricCharge";
            var expectedIncoming = 10;
            var expectedOutgoing = 7;
            var providedResources = new Dictionary<string, int>
            {
                { expectedResource, expectedIncoming }
            };
            var consumedResources = new Dictionary<string, int>
            {
                { expectedResource, expectedOutgoing }
            };
            depot.NegotiateProvider(providedResources);
            depot.NegotiateConsumer(consumedResources);

            persister.OnSave(configNode);

            Assert.True(configNode.HasNode(ScenarioPersister.SCENARIO_NODE_NAME));
            var wolfNode = configNode.GetNode(ScenarioPersister.SCENARIO_NODE_NAME);
            Assert.True(wolfNode.HasData);
            var depotNodes = wolfNode.GetNodes();
            var depotNode = depotNodes.First();
            Assert.True(depotNode.HasValue("Body"));
            Assert.True(depotNode.HasValue("Biome"));
            Assert.True(depotNode.HasValue("Situation"));
            var bodyValue = depotNode.GetValue("Body");
            var biomeVaue = depotNode.GetValue("Biome");
            var situationValue = (Vessel.Situations)Enum.Parse(typeof(Vessel.Situations), depotNode.GetValue("Situation"));
            Assert.Equal(expectedBody, bodyValue);
            Assert.Equal(expectedBiome, biomeVaue);
            Assert.Equal(expectedSituation, situationValue);
            Assert.True(depotNode.HasNode("RESOURCE"));
            var resourceNode = depotNode.GetNodes().First();
            Assert.True(resourceNode.HasValue("ResourceName"));
            Assert.True(resourceNode.HasValue("Incoming"));
            Assert.True(resourceNode.HasValue("Outgoing"));
            var nodeResourceName = resourceNode.GetValue("ResourceName");
            var nodeIncomingValue = int.Parse(resourceNode.GetValue("Incoming"));
            var nodeOutgoingValue = int.Parse(resourceNode.GetValue("Outgoing"));
            Assert.Equal(expectedResource, nodeResourceName);
            Assert.Equal(expectedIncoming, nodeIncomingValue);
            Assert.Equal(expectedOutgoing, nodeOutgoingValue);
        }

        [Fact]
        public void Should_be_able_to_load_depot_from_persistence()
        {
            var configNode = TestConfigNode.Node;
            var persister = new ScenarioPersister();
            var expectedBody = "Mun";
            var expectedBiome = "East Crater";
            var expectedSituation = Vessel.Situations.LANDED;
            var expectedResourceName = "ElectricCharge";
            var expectedIncomingQuantity = 37;
            var expectedOutgoingQuantity = 12;
            var expectedAvailableQuantity = expectedIncomingQuantity - expectedOutgoingQuantity;
            var consumedResources = new Dictionary<string, int>
            {
                { expectedResourceName, expectedAvailableQuantity }
            };

            persister.OnLoad(configNode);

            Assert.NotEmpty(persister.Depots);
            var depot = persister.Depots.First();
            Assert.Equal(expectedBody, depot.Body);
            Assert.Equal(expectedBiome, depot.Biome);
            Assert.Equal(expectedSituation, depot.Situation);
            var result = depot.NegotiateConsumer(consumedResources);
            Assert.IsType<OkNegotiationResult>(result);
        }
    }
}
