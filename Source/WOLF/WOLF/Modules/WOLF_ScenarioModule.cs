using System.Collections.Generic;
using USITools;

namespace WOLF
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.SPACECENTER, GameScenes.TRACKSTATION)]
    public class WOLF_ScenarioModule : ScenarioModule
    {
        private Configuration _configuration;
        public Configuration Configuration
        {
            get
            {
                return _configuration ?? (_configuration = GetConfiguration());
            }
        }

        public ServiceManager ServiceManager { get; private set; }

        private Configuration GetConfiguration()
        {
            var configNodes = GameDatabase.Instance.GetConfigNodes("WOLF_CONFIGURATION");
            var allowedResources = new List<string>();
            var blacklistedResources = new List<string>();

            foreach (var node in configNodes)
            {
                var configFromFile = ResourceUtilities.LoadNodeProperties<ConfigurationFromFile>(node);
                var resources = Configuration.ParseHarvestableResources(configFromFile.AllowedHarvestableResources);
                var blacklist = Configuration.ParseHarvestableResources(configFromFile.BlacklistedHomeworldResources);

                allowedResources.AddRange(resources);
                blacklistedResources.AddRange(blacklist);
            }

            var config = new Configuration();
            config.SetHarvestableResources(allowedResources);
            config.SetBlacklistedHomeworldResources(blacklistedResources);

            return config;
        }

        public override void OnAwake()
        {
            base.OnAwake();

            // Setup dependency injection for WOLF services
            var services = new ServiceCollection();
            services.AddSingletonService<IRegistryCollection, ScenarioPersister>();
            services.AddService<WOLF_PlanningMonitor>();
            services.AddService<WOLF_RouteMonitor>();

            ServiceManager = new ServiceManager(services);
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            var persister = ServiceManager.GetService<IRegistryCollection>();
            persister.OnLoad(node);
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            var persister = ServiceManager.GetService<IRegistryCollection>();
            persister.OnSave(node);
        }
    }
}
