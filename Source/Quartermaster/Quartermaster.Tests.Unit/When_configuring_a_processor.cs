using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Quartermaster.Tests.Unit
{
    [TestClass]
    public class When_configuring_a_processor
    {
        [TestMethod]
        public void A_processor_has_a_unique_id()
        {
            var thisProc = TestHelpers.GetProcessor();
            Assert.IsFalse(String.IsNullOrEmpty(thisProc.ProcessorId));
        }

        [TestMethod]
        public void A_Processor_has_a_vessel_id()
        {
            var thisProc = TestHelpers.GetProcessor();
            Assert.IsFalse(String.IsNullOrEmpty(thisProc.VesselId));
        }

        [TestMethod]
        public void A_recipe_can_be_assigned_to_a_job_slot_if_there_are_pool_resources_available()
        {
            var nw = new FakeNetwork();
            var thisProc = TestHelpers.GetProcessor();
            var thisRecipe = TestHelpers.GetSampleRecipeA();
            var thisPool = new ResourcePool(nw,"VESSEL","POOL");
            thisProc.ConnectToPool(thisPool);
            thisProc.AddAvailableRecipe(thisRecipe);
            var expected = true;
            var actual = thisProc.AssignRecipeToJob(thisRecipe, 0);
            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void A_recipe_cannot_be_assigned_to_a_job_slot_if_there_are_insufficient_pool_resources()
        {
            var nw = new FakeNetwork();
            TestHelpers.LoadNetwork();
            var thisProc = TestHelpers.GetProcessor();
            var thisRecipe = TestHelpers.GetSampleRecipeB();
            var thisPool = new ResourcePool(nw,"VESSEL", "POOL");
            thisProc.ConnectToPool(thisPool);
            thisProc.AddAvailableRecipe(thisRecipe);
            var expected = false;
            var actual = thisProc.AssignRecipeToJob(thisRecipe, 0);
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void Should_be_able_to_check_for_pool_resources_before_assigning_a_job_to_a_slot()
        {
            var nw = new FakeNetwork();
            var thisRecipe = new Recipe();
            var thisPool = new ResourcePool(nw,"VESSEL", "POOL");
            var expected = true;
            var actual = thisPool.CheckResources(thisRecipe);
            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void Assigning_a_job_slot_should_reduce_available_pool_resources()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Unloading_a_job_slot_should_free_up_allocated_resources_to_the_pool()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Replacing_a_job_slot_should_free_up_allocated_resources_to_the_pool()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void A_processor_can_load_a_series_of_recipes()
        {
            var thisProc = TestHelpers.GetProcessor();
            Recipe recA = TestHelpers.GetSampleRecipeA();
            Recipe recB = TestHelpers.GetSampleRecipeB();
            thisProc.AddAvailableRecipe(recA);
            thisProc.AddAvailableRecipe(recB);
            var aCount = thisProc.AvailableRecipes.Count;
            var eCount = 2;
            var arecA = thisProc.AvailableRecipes[0];
            var arecB = thisProc.AvailableRecipes[1];
            Assert.AreEqual(eCount,aCount);
            Assert.AreEqual(recA, arecA);
            Assert.AreEqual(recB, arecB);
        }

        [TestMethod]
        public void A_job_slot_will_not_have_a_recipe_by_default()
        {
            var thisProc = TestHelpers.GetProcessor();
            var thisJob = thisProc.LoadedJobs[0];
            var expected = false;
            var actual = thisJob.HasRecipe();
            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void Should_be_able_to_load_a_recipe_into_a_job_slot()
        {
            var thisProc = TestHelpers.GetProcessor();
            var thisJob = thisProc.LoadedJobs[0];
            var thisRecipe = new Recipe();
            thisJob.JobRecipe = thisRecipe;
            Assert.IsNotNull(thisJob.JobRecipe);
        }

        [TestMethod]
        public void Should_be_able_to_confirm_a_recipe_is_loaded_into_a_job_slot()
        {
            var thisProc = TestHelpers.GetProcessor();
            var thisJob = thisProc.LoadedJobs[0];
            var thisRecipe = new Recipe();
            thisJob.JobRecipe = thisRecipe;
            var expected = true;
            var actual = thisJob.HasRecipe();
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void Should_allow_partial_filling_of_job_slots()
        {
            var thisProc = TestHelpers.GetProcessor();
            thisProc.JobSlots = 2;
            thisProc.LoadedJobs[0].JobRecipe = new Recipe();
            Assert.IsTrue(thisProc.LoadedJobs[0].HasRecipe());
            Assert.IsFalse(thisProc.LoadedJobs[1].HasRecipe());
        }

        [TestMethod]
        public void A_recipe_can_be_assigned_to_more_than_one_slot()
        {
            var thisProc = TestHelpers.GetProcessor();
            thisProc.JobSlots = 2;
            var thisRecipe = new Recipe();
            thisProc.LoadedJobs[0].JobRecipe = thisRecipe;
            thisProc.LoadedJobs[1].JobRecipe = thisRecipe;
            var actualA = thisProc.LoadedJobs[0].JobRecipe;
            var actualB = thisProc.LoadedJobs[1].JobRecipe;
            Assert.AreEqual(thisRecipe, actualA);
            Assert.AreEqual(thisRecipe, actualB);
        }

        [TestMethod]
        public void A_Series_of_Job_Slots_can_have_different_recipes()
        {
            var thisProc = TestHelpers.GetProcessor();
            thisProc.JobSlots = 2;
            var recA = new Recipe();
            var recB = new Recipe();
            thisProc.LoadedJobs[0].JobRecipe = recA;
            thisProc.LoadedJobs[1].JobRecipe = recB;
            var actualA = thisProc.LoadedJobs[0].JobRecipe;
            var actualB = thisProc.LoadedJobs[1].JobRecipe;
            Assert.AreEqual(recA, actualA);
            Assert.AreEqual(recB, actualB);
        }

        [TestMethod]
        public void A_Processor_must_be_connected_to_a_single_Pool_for_inputs_and_outputs()
        {
            var nw = new FakeNetwork();
            var thisProcessor = TestHelpers.GetProcessor();
            var thisPool = new ResourcePool(nw,"VESSEL", "POOL");
            thisProcessor.ConnectToPool(thisPool);
            var expected = true;
            var actual = thisProcessor.IsConnectedToPool();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void A_Processor_is_not_connected_to_a_pool_by_default()
        {
            var thisProcessor = TestHelpers.GetProcessor();
            var expected = false;
            var actual = thisProcessor.IsConnectedToPool();
            Assert.AreEqual(expected,actual);
        }
    }
}