using System.Collections.Generic;
using System.Linq;

namespace WOLF
{
    public class ScenarioPersister : IRegistryCollection
    {
        private static readonly string STARTING_BODY = "Kerbin";
        private static readonly string STARTING_BIOME = "KSC";
        private static readonly Dictionary<string, int> STARTING_RESOURCES = new Dictionary<string, int>
        {
            { "Food", 10 },
            { "MaterialKits", 10 },
            { "Oxygen", 10 },
            { "Water", 10 }
        };
        public static readonly string DEPOTS_NODE_NAME = "DEPOTS";
        public static readonly string ROUTES_NODE_NAME = "ROUTES";

        protected List<IDepot> _depots { get; private set; } = new List<IDepot>();
        protected List<IRoute> _routes { get; private set; } = new List<IRoute>();

        public List<string> TransferResourceBlacklist { get; private set; } = new List<string>
        {
            "LifeSupport",
            "Habitation",
            "Power"
        };

        public IDepot CreateDepot(string body, string biome)
        {
            if (HasDepot(body, biome))
            {
                return GetDepot(body, biome);
            }

            var depot = new Depot(body, biome);
            _depots.Add(depot);

            return depot;
        }

        public IRoute CreateRoute(string originBody, string originBiome, string destinationBody, string destinationBiome, int payload)
        {
            // If neither depot exists, this will short-circuit because GetDepot will throw an exception
            var origin = GetDepot(originBody, originBiome);
            var destination = GetDepot(destinationBody, destinationBiome);

            // If a route already exists, increase its bandwidth
            var existingRoute = GetRoute(originBody, originBiome, destinationBody, destinationBiome);
            if (existingRoute != null)
            {
                existingRoute.IncreasePayload(payload);
                return existingRoute;
            }

            var route = new Route(
                originBody,
                originBiome,
                destinationBody,
                destinationBiome,
                payload,
                this);

            _routes.Add(route);

            return route;
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
            return _depots.ToList() ?? new List<IDepot>();
        }

        public IRoute GetRoute(string originBody, string originBiome, string destinationBody, string destinationBiome)
        {
            return _routes
                .Where(r => r.OriginBody == originBody
                    && r.OriginBiome == originBiome
                    && r.DestinationBody == destinationBody
                    && r.DestinationBiome == destinationBiome)
                .FirstOrDefault();
        }

        public List<IRoute> GetRoutes()
        {
            return _routes.ToList() ?? new List<IRoute>();
        }

        public bool HasDepot(string body, string biome)
        {
            return _depots.Any(d => d.Body == body && d.Biome == biome);
        }

        public bool HasRoute(string originBody, string originBiome, string destinationBody, string destinationBiome)
        {
            return _routes
                .Any(r => r.OriginBody == originBody
                    && r.OriginBiome == originBiome
                    && r.DestinationBody == destinationBody
                    && r.DestinationBiome == destinationBiome);
        }

        public void OnLoad(ConfigNode node)
        {
            if (node.HasNode(DEPOTS_NODE_NAME))
            {
                var wolfNode = node.GetNode(DEPOTS_NODE_NAME);
                var depotNodes = wolfNode.GetNodes();
                foreach (var depotNode in depotNodes)
                {
                    var bodyValue = depotNode.GetValue("Body");
                    var biomeValue = depotNode.GetValue("Biome");

                    var depot = new Depot(bodyValue, biomeValue);
                    depot.OnLoad(depotNode);
                    _depots.Add(depot);
                }
                if (_depots.Count < 1)
                {
                    // Setup a starting depot on Kerbin
                    var starterDepot = CreateDepot(STARTING_BODY, STARTING_BIOME);
                    starterDepot.Establish();
                    starterDepot.NegotiateProvider(STARTING_RESOURCES);
                }
            }
            if (node.HasNode(ROUTES_NODE_NAME))
            {
                var wolfNode = node.GetNode(ROUTES_NODE_NAME);
                var routeNodes = wolfNode.GetNodes();
                foreach (var routeNode in routeNodes)
                {
                    var route = new Route(this);
                    route.OnLoad(routeNode);

                    _routes.Add(route);
                }
            }
        }

        public void OnSave(ConfigNode node)
        {
            ConfigNode depotsNode;
            if (!node.HasNode(DEPOTS_NODE_NAME))
            {
                depotsNode = node.AddNode(DEPOTS_NODE_NAME);
            }
            else
            {
                depotsNode = node.GetNode(DEPOTS_NODE_NAME);
            }

            ConfigNode routesNode;
            if (!node.HasNode(ROUTES_NODE_NAME))
            {
                routesNode = node.AddNode(ROUTES_NODE_NAME);
            }
            else
            {
                routesNode = node.GetNode(ROUTES_NODE_NAME);
            }

            foreach (var depot in _depots)
            {
                depot.OnSave(depotsNode);
            }

            foreach (var route in _routes)
            {
                route.OnSave(routesNode);
            }
        }
    }
}
