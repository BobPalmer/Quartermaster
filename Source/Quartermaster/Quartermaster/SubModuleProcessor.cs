using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace Quartermaster
{
    public class SubModuleProcessor
    {
        private List<Job> _loadedJobs;
        private List<Recipe> _availableRecipes;
        private int _jobSlots;
        private IGameData _gameData;
        private IResourceNetworkProvider _network;


        public string VesselId
        {
            get { return _vesselId; }
            set { _vesselId = value; }
        }

        public string ProcessorId
        {
            get { return _processorId; }
            set { _processorId = value; }
        }

        public SubModuleProcessor(string vId, string pId) : this(new KSPGameData(), ResourceNetwork.Instance, vId, pId)
        { }

        public SubModuleProcessor(IGameData gd, IResourceNetworkProvider net, string vId, string pId)
        {
            _gameData = gd;
            _network = net;
            VesselId = vId;
            ProcessorId = pId;
        }

        public int JobSlots
        {
            get { return _jobSlots; }
            set
            {
                _jobSlots = Math.Max(1,value);
            }
        }

        public void AddAvailableRecipe(Recipe recipe)
        {
            AvailableRecipes.Add(recipe);
        }

        public List<Job> LoadedJobs
        {
            get
            {
                if (_loadedJobs == null)
                    SetupJobSlots();
                return _loadedJobs;
            }
        }

        public List<Recipe> AvailableRecipes
        {
            get
            {
                if (_availableRecipes == null)
                    _availableRecipes = new List<Recipe>();
                return _availableRecipes;
            }
        }

        private void SetupJobSlots()
        {
            //Guard clause
            if(JobSlots < 1)
                JobSlots = 1;

            _loadedJobs= new List<Job>();
            for (int i = 0; i < JobSlots; ++i)
            {
                _loadedJobs.Add(new Job());
            }
        }

        public bool IsConnectedToPool()
        {
            return _resPool != null;
        }

        private ResourcePool _resPool;
        private string _vesselId;
        private string _processorId;

        public void ConnectToPool(ResourcePool pool)
        {
            _resPool = pool;
        }

        public void ExecuteJobs()
        {
            if (!_gameData.LoadedSceneIsFlight())
                return;

            if (!IsConnectedToPool())
                return;

            for (int i = 0; i < JobSlots; ++i)
            {
                var thisJob = LoadedJobs[i];
                if (!thisJob.HasRecipe())
                    continue;
                RunJob(thisJob);
            }
        }

        private void RunJob(Job job)
        {
            job.SetLastUpdate(_gameData.GetUniversalTime());
        }

        public bool AssignRecipeToJob(Recipe r, int idx)
        {
            var canLoad = _resPool.CheckResources(r);
            if (canLoad)
            {
                LoadedJobs[idx].JobRecipe = r;
                return true;
            }
            //If there are insufficient resources, the recipe is not loaded.
            return false;
        }
    }
}
