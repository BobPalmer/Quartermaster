using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quartermaster.Tests.Unit
{
    [TestClass]
    public class When_Exploring_Endpoints
    {
        [TestMethod]
        public void Should_be_able_to_check_if_an_endpoint_exists()
        {
            var actual = FakeNetwork.Instance.Repo.EndpointExists("POOL-01");
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void Should_be_able_to_check_if_an_endpoint_does_not_exist()
        {
            var actual = FakeNetwork.Instance.Repo.EndpointExists("POOL-99");
            Assert.IsFalse(actual);
        }
    }
}