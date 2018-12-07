using USITools;

namespace WOLF
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.SPACECENTER, GameScenes.TRACKSTATION)]
    public class WOLF_ScenarioModule : ScenarioModule
    {
        public ServiceManager ServiceManager { get; private set; }

        public override void OnAwake()
        {
            base.OnAwake();

            // Setup dependency injection for WOLF services
            var services = new ServiceCollection();
            services.AddSingletonService<IDepotRegistry, ScenarioPersister>();

            ServiceManager = new ServiceManager(services);
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            var persister = ServiceManager.GetService<IDepotRegistry>();
            persister.OnLoad(node);
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            var persister = ServiceManager.GetService<IDepotRegistry>();
            persister.OnSave(node);
        }
    }
}
