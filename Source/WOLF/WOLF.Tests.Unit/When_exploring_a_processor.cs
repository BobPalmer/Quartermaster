using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quartermaster.Tests.Unit
{
    [TestClass]
    public class When_exploring_a_processor
    {
        [TestMethod]
        public void Should_be_able_to_create_a_new_processor()
        {
            var thisProc = TestHelpers.GetProcessor();
            Assert.IsNotNull(thisProc);
        }

        [TestMethod]
        public void a_processor_must_have_at_least_one_job_slot()
        {
            var proc = TestHelpers.GetProcessor();
            var expected = 1;
            var actual = proc.LoadedJobs.Count;
            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void a_processor_can_have_several_job_slots()
        {
            var proc = TestHelpers.GetProcessor();
            proc.JobSlots = 3;
            var expected = 3;
            var actual = proc.LoadedJobs.Count;
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void A_processor_can_have_at_least_one_valid_recipe()
        {
            var proc = TestHelpers.GetProcessor();
            proc.AddAvailableRecipe(new Recipe());
            var expected = 1;
            var actual = proc.AvailableRecipes.Count;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void A_processor_can_have_several_valid_recipes()
        {
            var proc = TestHelpers.GetProcessor();
            proc.AddAvailableRecipe(new Recipe());
            proc.AddAvailableRecipe(new Recipe());
            proc.AddAvailableRecipe(new Recipe());
            var expected = 3;
            var actual = proc.AvailableRecipes.Count;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void A_job_is_initialized_with_a_lastUpdate_of_negative_one()
        {
            var proc = TestHelpers.GetProcessor();
            var job = proc.LoadedJobs[0];
            var ex = -1d;
            var actual = job.LastUpdate;

            Assert.AreEqual(ex,actual);
        }

    }
}