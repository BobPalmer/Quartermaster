using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quartermaster.Tests.Unit
{
    [TestClass]
    public class When_exploring_The_Resource_Network
    {
        [TestMethod]
        public void Should_be_able_to_retrieve_an_existing_resource_link()
        {
            var link2 = TestHelpers.Network.Repo.FetchLink("RL0013");
            Assert.IsNotNull(link2);
        }

        [TestMethod]
        public void Should_be_able_to_see_if_a_resource_link_exists()
        {
            var link1 = TestHelpers.Network.Repo.FetchLink("BADLINK");
            Assert.IsNull(link1);
            var link2 = TestHelpers.Network.Repo.FetchLink("RL0013");
            Assert.IsNotNull(link2);
        }

        [TestMethod]
        public void Should_be_able_to_clear_dead_links()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Should_be_able_to_add_a_resource_link()
        {
            var count = TestHelpers.Network.Repo.RecordCount();
            var expected = count + 1;
            TestHelpers.Network.Repo.SaveLink(new ResourceLink());
            var actual = TestHelpers.Network.Repo.RecordCount();
            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void Should_have_an_empty_id_on_link_creation()
        {
            var link = new ResourceLink();
            Assert.IsTrue(string.IsNullOrEmpty(link.LinkId));
        }

        [TestMethod]
        public void Should_create_a_link_id_automatically_on_save()
        {
            var link = new ResourceLink();
            TestHelpers.Network.Repo.SaveLink(link);
            Assert.IsFalse(string.IsNullOrEmpty(link.LinkId));
        }

        [TestMethod]
        public void Should_not_change_a_link_id()
        {
            var link = new ResourceLink();
            link.LinkId = "TEST";
            TestHelpers.Network.Repo.SaveLink(link);
            var expected = "TEST";
            var actual = link.LinkId;
            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void Should_be_able_to_delete_a_resource_link()
        {
            var count = TestHelpers.Network.Repo.RecordCount();
            var id = TestHelpers.Network.Repo.SaveLink(new ResourceLink());
            var expected = count + 1;
            var actual = TestHelpers.Network.Repo.RecordCount();
            Assert.AreEqual(expected, actual);

            TestHelpers.Network.Repo.DeleteLink(id);
            expected = count;
            actual = TestHelpers.Network.Repo.RecordCount();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Should_be_able_to_calculate_resource_totals_for_an_endpoint()
        {
            var expected = 2;
            var actual = TestHelpers.Network.Repo.GetResourceQuantity("POOL-03", "LFO");
            Assert.AreEqual(expected,actual);
        }

    }
}