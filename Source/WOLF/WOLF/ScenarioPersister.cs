namespace WOLF
{
    public class ScenarioPersister : IPersistenceAware
    {
        private IContractNegotiator _contractNegotiator;
        private static string _scenarioNodeName = "WOLF_DATA";

        public ScenarioPersister(IContractNegotiator contractNegotiator)
        {
            _contractNegotiator = contractNegotiator;
        }

        public void OnLoad(ConfigNode node)
        {
            if (node.HasNode(_scenarioNodeName))
            {
                var scenarioNode = node.GetNode(_scenarioNodeName);

                _contractNegotiator.OnLoad(scenarioNode);
            }
        }

        public void OnSave(ConfigNode node)
        {
            var scenarioNode = node.HasNode(_scenarioNodeName)
                ? node.GetNode(_scenarioNodeName)
                : node.AddNode(_scenarioNodeName);

            _contractNegotiator.OnSave(scenarioNode);
        }
    }
}
