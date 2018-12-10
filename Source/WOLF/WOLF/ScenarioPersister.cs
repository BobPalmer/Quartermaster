using System.Collections.Generic;
using System.Linq;

namespace WOLF
{
    public class ScenarioPersister : IDepotRegistry
    {
        public static readonly string SCENARIO_NODE_NAME = "WOLF_DATA";
        protected List<IDepot> _depots { get; private set; } = new List<IDepot>();

        public IDepot AddDepot(string body, string biome)
        {
            if (HasDepot(body, biome))
            {
                return GetDepot(body, biome);
            }

            var depot = new Depot(body, biome);
            _depots.Add(depot);

            return depot;
        }

        public bool HasDepot(string body, string biome)
        {
            return _depots.Any(d => d.Body == body && d.Biome == biome);
        }

        public IDepot GetDepot(string body, string biome)
        {
            var depot = _depots.Where(d => d.Body == body && d.Biome == biome).FirstOrDefault();

            if (depot == null)
            {
                throw new DepotDoesNotExistException(body, biome);
            }

            return depot;
        }

        public List<IDepot> GetDepots()
        {
            return _depots.ToList();
        }

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

                    var depot = new Depot(bodyValue, biomeValue);
                    depot.OnLoad(depotNode);
                    _depots.Add(depot);
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

            foreach (var depot in _depots)
            {
                depot.OnSave(wolfNode);
            }
        }
    }
}
