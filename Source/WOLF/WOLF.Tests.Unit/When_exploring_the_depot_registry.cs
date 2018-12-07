using System.Linq;
using WOLF.Tests.Unit.Mocks;
using Xunit;

namespace WOLF.Tests.Unit
{
    public class When_exploring_the_depot_registry
    {
        [Fact]
        public void Can_add_depot_to_registry()
        {
            var registry = new TestPersister();
            var expectedBody = "Mun";
            var expectedBiome = "East Crater";

            var depot = registry.AddDepot(expectedBody, expectedBiome);

            Assert.Contains(depot, registry.Depots);
        }

        [Fact]
        public void Can_find_a_depot()
        {
            var registry = new TestPersister();
            var expectedBody = "Mun";
            var expectedBiome = "East Crater";
            registry.AddDepot(expectedBody, expectedBiome);

            var hasDepot = registry.HasDepot(expectedBody, expectedBiome);
            var depot = registry.GetDepot(expectedBody, expectedBiome);

            Assert.True(hasDepot);
            Assert.Equal(expectedBody, depot.Body);
            Assert.Equal(expectedBiome, depot.Biome);
        }

        [Fact]
        public void Can_find_all_depots()
        {
            throw new System.NotImplementedException();
        }

        [Fact]
        public void Should_not_allow_multiple_depots_in_the_same_biome()
        {
            var registry = new TestPersister();
            var expectedBody = "Mun";
            var expectedBiome = "East Crater";
            var firstDepot = registry.AddDepot(expectedBody, expectedBiome);

            var secondDepot = registry.AddDepot(expectedBody, expectedBiome);

            Assert.Equal(firstDepot, secondDepot);
            var depots = registry.Depots.Where(d => d.Body == expectedBody && d.Biome == expectedBiome);
            Assert.True(depots.Count() == 1);
        }
    }
}
