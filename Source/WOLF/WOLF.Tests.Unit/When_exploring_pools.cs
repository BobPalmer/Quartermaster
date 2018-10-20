using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quartermaster.Tests.Unit
{
    [TestClass]
    public class When_exploring_pools
    {
        [TestMethod]
        public void Should_be_able_to_obtain_a_list_of_pools()
        {
            var nw = new FakeNetwork();
            var poolList = nw.Repo.GetPoolDisplayList();
            var expected = 5;
            Assert.AreEqual(expected,poolList.Count);
        }

        [TestMethod]
        public void Should_be_able_to_obtain_a_list_of_resources_for_a_pool()
        {
            var nw = new FakeNetwork();
            var poolList = nw.Repo.GetPoolDisplayDetails("POOL-01");
            var expected = 3;
            Assert.AreEqual(expected, poolList.Count);
        }

        [TestMethod]
        public void Should_be_able_to_check_a_pools_capacity()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Adding_to_a_pool_decreses_capacity()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_be_able_to_create_a_ResourcePool()
        {
            var nw = new FakeNetwork();
            var thisPool = new ResourcePool(nw,"VESSEL", "POOL", true, 0,EndpointTypes.Pool);
            Assert.IsNotNull(thisPool);
        }

        [TestMethod]
        public void Should_be_able_to_check_for_all_present_resources()
        {
            var nw = new FakeNetwork();
            var thisPool = new ResourcePool(nw,"VESSEL", "POOL", true, 0,EndpointTypes.Pool);
            var thisRecipe = TestHelpers.GetSampleRecipeA();
            var expected = true;
            var actual = thisPool.CheckResources(thisRecipe);
            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void Should_be_able_to_check_for_insufficient_capacity()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_persist_a_pool_automatically_upon_creation()
        {
            var epNum = FakeNetwork.Instance.Repo.Endpoints.Count;
            var p = new ResourcePool(FakeNetwork.Instance, "NewVessel","NewPool", true, 0,EndpointTypes.Pool);
            var expected = epNum + 1;
            var actual = FakeNetwork.Instance.Repo.Endpoints.Count;
            Assert.AreEqual(expected,actual);
        }


        [TestMethod]
        public void Should_be_able_to_check_for_missing_requirements()
        {
            var nw = new FakeNetwork();
            var thisPool = new ResourcePool(nw,"VESSEL", "POOL", true, 0,EndpointTypes.Pool);
            var thisRecipe = TestHelpers.GetSampleRecipeB();
            var expected = false;
            var actual = thisPool.CheckResources(thisRecipe);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Should_be_able_to_check_for_missing_inputs()
        {
            var nw = new FakeNetwork();
            var thisPool = new ResourcePool(nw,"VESSEL", "POOL", true, 0,EndpointTypes.Pool);
            var thisRecipe = TestHelpers.GetSampleRecipeB();
            var expected = false;
            var actual = thisPool.CheckResources(thisRecipe);
            Assert.AreEqual(expected, actual);
        }
    }
}