namespace WOLF.Tests.Unit.Mocks
{
    public static class TestConfigNode
    {
        public static ConfigNode Node { get; private set; }

        static TestConfigNode()
        {
            Node = new ConfigNode();
            var scenarioNode = Node.AddNode(ScenarioPersister.SCENARIO_NODE_NAME);
            var depotNode = scenarioNode.AddNode(TestDepot.DEPOT_NODE_NAME);
            depotNode.AddValue("Body", "Mun");
            depotNode.AddValue("Biome", "East Crater");
            depotNode.AddValue("Situation", "LANDED");
            var streamNode = depotNode.AddNode(TestDepot.STREAM_NODE_NAME);
            streamNode.AddValue("ResourceName", "Ore");
            streamNode.AddValue("Incoming", 100);
            streamNode.AddValue("Outgoing", 78);
            streamNode = depotNode.AddNode(TestDepot.STREAM_NODE_NAME);
            streamNode.AddValue("ResourceName", "ElectricCharge");
            streamNode.AddValue("Incoming", 37);
            streamNode.AddValue("Outgoing", 12);
        }
    }
}
