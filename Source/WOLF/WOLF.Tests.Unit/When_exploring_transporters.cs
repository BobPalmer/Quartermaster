using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quartermaster.Tests.Unit
{
    [TestClass]
    public class When_exploring_transporters
    {
        [TestMethod]
        public void Should_be_able_to_create_a_resource_transporter()
        {
            var trans = new ResourceTransporter();
        }
    }

}