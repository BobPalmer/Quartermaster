using System.Collections.Generic;
using WOLF.Tests.Unit.Mocks;
using Xunit;

namespace WOLF.Tests.Unit
{
    public class When_exploring_depots
    {
        [Fact]
        public void Should_have_no_resource_streams_by_default()
        {
            var depot = new TestDepot();

            Assert.NotNull(depot.Resources);
            Assert.Empty(depot.Resources);
        }

        [Fact]
        public void Can_show_resource_streams()
        {
            throw new System.NotImplementedException();
        }

        [Fact]
        public void Can_negotiate_a_provider_relationship()
        {
            var depot = new TestDepot();
            var expectedResource = "ElectricCharge";
            var expectedQuantity = 10;
            var providedResources = new Dictionary<string, int>
            {
                { expectedResource, expectedQuantity }
            };

            var result = depot.NegotiateProvider(providedResources);

            Assert.IsType<OkNegotiationResult>(result);
            Assert.NotEmpty(depot.Resources);
            var stream = depot.Resources[expectedResource];
            Assert.NotNull(stream);
            Assert.Equal(expectedResource, stream.ResourceName);
            Assert.Equal(expectedQuantity, stream.Incoming);
            Assert.Equal(expectedQuantity, stream.Available);
        }

        [Fact]
        public void Can_negotiate_a_consumer_relationship()
        {
            var depot = new TestDepot();
            var resourceName = "ElectricCharge";
            var providedQuantity = 10;
            var requestedQuantity = 3;
            var expectedRemainingQuantity = 7;
            var providedResources = new Dictionary<string, int>
            {
                { resourceName, providedQuantity }
            };
            var consumedResources = new Dictionary<string, int>
            {
                { resourceName, requestedQuantity }
            };
            depot.NegotiateProvider(providedResources);

            var result = depot.NegotiateConsumer(consumedResources);

            Assert.IsType<OkNegotiationResult>(result);
            var stream = depot.Resources[resourceName];
            Assert.Equal(requestedQuantity, stream.Outgoing);
            Assert.Equal(expectedRemainingQuantity, stream.Available);
        }

        [Fact]
        public void Can_negotiate_a_relationship_for_a_recipe()
        {
            var depot = new TestDepot();
            var consumedResource1 = "ElectricCharge";
            var consumedResource2 = "Ore";
            var providedResource1 = "LiquidFuel";
            var consumedQuantity1 = 10;
            var consumedQuantity2 = 10;
            var providedQuantity1 = 5;
            var startingResources = new Dictionary<string, int>
            {
                { consumedResource1, 15 },
                { consumedResource2, 10 }
            };
            depot.NegotiateProvider(startingResources);
            var recipe = new Recipe();
            recipe.InputIngredients.Add(consumedResource1, consumedQuantity1);
            recipe.InputIngredients.Add(consumedResource2, consumedQuantity2);
            recipe.OutputIngredients.Add(providedResource1, providedQuantity1);
            var expectedRemainingEC = 5;
            var expectedRemainingOre = 0;

            var result = depot.Negotiate(recipe);

            Assert.IsType<OkNegotiationResult>(result);
            var ecStream = depot.Resources[consumedResource1];
            var oreStream = depot.Resources[consumedResource2];
            var lfStream = depot.Resources[providedResource1];
            Assert.Equal(consumedQuantity1, ecStream.Outgoing);
            Assert.Equal(expectedRemainingEC, ecStream.Available);
            Assert.Equal(consumedQuantity2, oreStream.Outgoing);
            Assert.Equal(expectedRemainingOre, oreStream.Available);
            Assert.Equal(providedQuantity1, lfStream.Incoming);
            Assert.Equal(providedQuantity1, lfStream.Available);
        }

        [Fact]
        public void Can_negotiate_a_relationship_for_multiple_dependent_recipes()
        {
            throw new System.NotImplementedException();
        }

        [Theory]
        [InlineData("Ore", 1, 2, true)]
        [InlineData("Ore", 0, 2, false)]
        public void Should_not_allow_contracts_if_resources_are_not_available(string resourceName, int availableQuantity, int requestedQuantity, bool resourceExists)
        {
            var depot = new TestDepot();
            if (resourceExists)
            {
                var providedResources = new Dictionary<string, int>
                {
                    { resourceName, availableQuantity }
                };
                depot.NegotiateProvider(providedResources);
            }
            var consumedResources = new Dictionary<string, int>
            {
                { resourceName, requestedQuantity }
            };

            var result = depot.NegotiateConsumer(consumedResources);

            var failedResult = Assert.IsType<FailedNegotiationResult>(result);
            Assert.Contains(failedResult.MissingResources, r => r.Key == resourceName && r.Value == requestedQuantity - availableQuantity);
        }
    }
}
