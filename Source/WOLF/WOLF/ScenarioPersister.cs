using System;
using System.Collections.Generic;

namespace WOLF
{
    public class ScenarioPersister : IPersistenceAware
    {
        public static readonly string SCENARIO_NODE_NAME = "WOLF_DATA";
        public List<IDepot> Depots = new List<IDepot>();

        public void OnLoad(ConfigNode node)
        {
            if (node.HasNode(SCENARIO_NODE_NAME))
            {
                var wolfNode = node.GetNode(SCENARIO_NODE_NAME);
                var depotNodes = wolfNode.GetNodes();
                foreach (var depotNode in depotNodes)
                {
                    var bodyValue = depotNode.GetValue("Body");
                    var biomeValue = depotNode.GetValue("Biome");
                    var situationValue = (Vessel.Situations)Enum.Parse(typeof(Vessel.Situations), depotNode.GetValue("Situation"));

                    var depot = new Depot(bodyValue, biomeValue, situationValue);
                    depot.OnLoad(depotNode);
                    Depots.Add(depot);
                }
            }
        }

        public void OnSave(ConfigNode node)
        {
            ConfigNode wolfNode;
            if (!node.HasNode("WOLF_DATA"))
            {
                wolfNode = node.AddNode(SCENARIO_NODE_NAME);
            }
            else
            {
                wolfNode = node.GetNode(SCENARIO_NODE_NAME);
            }

            foreach (var depot in Depots)
            {
                depot.OnSave(wolfNode);
            }
        }
    }
}
