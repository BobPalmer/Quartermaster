namespace Quartermaster
{
    public class ResourceNetworkScenario : ScenarioModule
    {
        public ResourceNetworkScenario()
        {
            Instance = this;
            ResourceLinks = new ResourcePersistence();
        }

        public static ResourceNetworkScenario Instance { get; private set; }
        public ResourcePersistence  ResourceLinks { get; private set; }

        public override void OnLoad(ConfigNode gameNode)
        {
            base.OnLoad(gameNode);
            ResourceLinks.Load(gameNode);
        }

        public override void OnSave(ConfigNode gameNode)
        {
            base.OnSave(gameNode);
            ResourceLinks.Save(gameNode);
        }
    }
}