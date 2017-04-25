using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;

namespace Quartermaster.Tests.Unit
{
    [TestClass]
    public class When_executing_a_job_on_a_processor
    {
        [TestMethod]
        public void Jobs_cannot_be_executed_if_the_resource_pool_is_not_set()
        {
            var gi = new FakeGameInterface();
            var nw = new FakeNetwork();

            gi.SetUniversalTime(300000d);
            gi.SetFlightState(true);
            var thisProc = new SubModuleProcessor(gi,nw,"VESSEL","PROCESSOR");
            var thisRecipe = new Recipe();
            thisProc.JobSlots = 2;
            thisProc.LoadedJobs[0].JobRecipe = thisRecipe;
            thisProc.LoadedJobs[1].JobRecipe = thisRecipe;
            var exA = -1d;
            var exB = -1d;
            thisProc.ExecuteJobs();
            var actA = thisProc.LoadedJobs[0].LastUpdate;
            var actB = thisProc.LoadedJobs[1].LastUpdate;
            Assert.AreEqual(exA, actA);
            Assert.AreEqual(exB, actB);
        }

        [TestMethod]
        public void A_job_slot_without_a_recipe_will_not_execute()
        {
            var gi = new FakeGameInterface();
            var nw = new FakeNetwork();

            gi.SetUniversalTime(100000d);
            gi.SetFlightState(true);
            var thisProc = new SubModuleProcessor(gi,nw, "VESSEL", "PROCESSOR");
            var thisRecipe = new Recipe();
            var thisPool = new ResourcePool(nw,"VESSEL", "POOL");
            thisProc.ConnectToPool(thisPool);
            thisProc.JobSlots = 2;
            thisProc.LoadedJobs[1].JobRecipe = thisRecipe;
            thisProc.ExecuteJobs();
            var exA = -1d;
            var exB = 100000d;
            var actA = thisProc.LoadedJobs[0].LastUpdate;
            var actB = thisProc.LoadedJobs[1].LastUpdate;
            Assert.AreEqual(exA, actA);
            Assert.AreEqual(exB, actB);
        }

        [TestMethod]
        public void Should_only_execute_jobs_in_flight()
        {
            var gi = new FakeGameInterface();
            var nw = new FakeNetwork();

            gi.SetUniversalTime(200000d);
            gi.SetFlightState(false);
            var thisProc = new SubModuleProcessor(gi,nw, "VESSEL", "PROCESSOR");
            var thisRecipe = new Recipe();
            thisProc.JobSlots = 2;
            thisProc.LoadedJobs[0].JobRecipe = thisRecipe;
            thisProc.LoadedJobs[1].JobRecipe = thisRecipe;
            thisProc.ExecuteJobs();
            var exA = -1d;
            var exB = -1d;
            var actA = thisProc.LoadedJobs[0].LastUpdate;
            var actB = thisProc.LoadedJobs[1].LastUpdate;
            Assert.AreEqual(exA, actA);
            Assert.AreEqual(exB, actB);
        }

        [TestMethod]
        public void Should_execute_all_jobs()
        {
            var gi = new FakeGameInterface();
            var nw = new FakeNetwork();

            gi.SetUniversalTime(300000d);
            gi.SetFlightState(true);
            var thisProc = new SubModuleProcessor(gi,nw, "VESSEL", "PROCESSOR");
            var thisRecipe = new Recipe();
            var thisPool = new ResourcePool(nw,"VESSEL", "POOL");
            thisProc.ConnectToPool(thisPool);
            thisProc.JobSlots = 2;
            thisProc.LoadedJobs[0].JobRecipe = thisRecipe;
            thisProc.LoadedJobs[1].JobRecipe = thisRecipe;
            var exA = 300000d;
            var exB = 300000d;
            thisProc.ExecuteJobs();
            var actA = thisProc.LoadedJobs[0].LastUpdate;
            var actB = thisProc.LoadedJobs[1].LastUpdate;
            Assert.AreEqual(exA, actA);
            Assert.AreEqual(exB, actB);
        }

    }
}