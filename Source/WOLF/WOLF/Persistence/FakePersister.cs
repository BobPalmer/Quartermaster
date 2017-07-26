using System.Collections.Generic;

namespace Quartermaster.Persistence
{
    public class FakePersister : INetworkPersister
    {
        public List<ResourceLink> GetLinkInfo()
        {
            var demoLinks = new List<ResourceLink>();
            demoLinks.Add(new ResourceLink{ LinkId = "RL0001",SourceId = "CON-01",DestinationId = "POOL-01",ResourceName = "Ore",Quantity = 10});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0002",SourceId = "POOL-01",DestinationId = "CON-01",ResourceName = "EC",Quantity = 1});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0003",SourceId = "CON-2",DestinationId = "POOL-01",ResourceName = "LFO",Quantity = 10});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0004",SourceId = "POOL-01",DestinationId = "CON-02",ResourceName = "EC",Quantity = 1});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0005",SourceId = "POOL-01",DestinationId = "CON-02",ResourceName = "Ore",Quantity = 10});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0006",SourceId = "CON-03",DestinationId = "POOL-02",ResourceName = "Rares",Quantity = 5});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0007",SourceId = "POOL-02",DestinationId = "CON-03",ResourceName = "EC",Quantity = 1});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0008",SourceId = "CON-04",DestinationId = "POOL-03",ResourceName = "EC",Quantity = 3});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0009",SourceId = "CON-05",DestinationId = "POOL-04",ResourceName = "TR",Quantity = 7});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0010",SourceId = "POOL-04",DestinationId = "CON-05",ResourceName = "FUN",Quantity = 7});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0011",SourceId = "POOL-04",DestinationId = "HOP-02",ResourceName = "FUN",Quantity = 3});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0012",SourceId = "CON-06",DestinationId = "POOL-04",ResourceName = "FUN",Quantity = 10});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0013",SourceId = "POOL-04",DestinationId = "CON-06",ResourceName = "Rares",Quantity = 5});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0014",SourceId = "POOL-03",DestinationId = "HOP-01",ResourceName = "LFO",Quantity = 1});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0015",SourceId = "POOL-05",DestinationId = "TRANS-01",ResourceName = "LFO",Quantity = 7});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0016",SourceId = "POOL-05",DestinationId = "TRANS-01",ResourceName = "TR",Quantity = 7});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0017",SourceId = "POOL-03",DestinationId = "POOL-01",ResourceName = "EC",Quantity = 2});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0018",SourceId = "POOL-03",DestinationId = "POOL-02",ResourceName = "EC",Quantity = 1});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0019",SourceId = "POOL-01",DestinationId = "POOL-03",ResourceName = "LFO",Quantity = 10});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0020",SourceId = "POOL-03",DestinationId = "POOL-05",ResourceName = "LFO",Quantity = 7});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0021",SourceId = "POOL-02",DestinationId = "POOL-05",ResourceName = "Rares",Quantity = 5});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0022",SourceId = "POOL-05",DestinationId = "POOL-04",ResourceName = "Rares",Quantity = 5});
            demoLinks.Add(new ResourceLink{ LinkId = "RL0023",SourceId = "POOL-04",DestinationId = "POOL-05",ResourceName = "TR",Quantity = 7});
            return demoLinks;
        }

        public void DeleteLinkNode(string id)
        {
        }

        public void SaveLinkNode(ResourceLink link)
        {
        }

        public List<Route> GetRouteInfo()
        {
            var demoRoutes = new List<Route>();
            demoRoutes.Add(new Route { EndpointId = "TRANS-01", RouteId = "Route-A", LinkId = "RL0017" });
            demoRoutes.Add(new Route { EndpointId = "TRANS-01", RouteId = "Route-B", LinkId = "RL0018" });
            demoRoutes.Add(new Route { EndpointId = "TRANS-01", RouteId = "Route-C", LinkId = "RL0019" });
            demoRoutes.Add(new Route { EndpointId = "TRANS-01", RouteId = "Route-D", LinkId = "RL0020" });
            demoRoutes.Add(new Route { EndpointId = "TRANS-01", RouteId = "Route-E", LinkId = "RL0021" });
            demoRoutes.Add(new Route { EndpointId = "TRANS-01", RouteId = "Route-F", LinkId = "RL0022" });
            demoRoutes.Add(new Route { EndpointId = "TRANS-01", RouteId = "Route-G", LinkId = "RL0023" });
            return demoRoutes;
        }

        public void DeleteRouteNode(string id)
        {
        }

        public void SaveRouteNode(Route route)
        {
        }

        public List<Endpoint> GetEndpointInfo()
        {
            var demoEp = new List<Endpoint>();
            demoEp.Add(new Endpoint { EndpointId = "TRANS-01", VesselId = "V-03" });
            demoEp.Add(new Endpoint { EndpointId = "CON-01", VesselId = "V-01" });
            demoEp.Add(new Endpoint { EndpointId = "CON-02", VesselId = "V-01" });
            demoEp.Add(new Endpoint { EndpointId = "CON-03", VesselId = "V-01" });
            demoEp.Add(new Endpoint { EndpointId = "CON-04", VesselId = "V-04" });
            demoEp.Add(new Endpoint { EndpointId = "CON-05", VesselId = "K" });
            demoEp.Add(new Endpoint { EndpointId = "CON-06", VesselId = "K" });
            demoEp.Add(new Endpoint { EndpointId = "POOL-01", VesselId = "V-02" });
            demoEp.Add(new Endpoint { EndpointId = "POOL-02", VesselId = "V-05" });
            demoEp.Add(new Endpoint { EndpointId = "POOL-03", VesselId = "V-04" });
            demoEp.Add(new Endpoint { EndpointId = "POOL-04", VesselId = "K" });
            demoEp.Add(new Endpoint { EndpointId = "POOL-05", VesselId = "V-03" });
            demoEp.Add(new Endpoint { EndpointId = "HOP-01", VesselId = "V-04" });
            demoEp.Add(new Endpoint { EndpointId = "HOP-02", VesselId = "K" });
            return demoEp;
        }

        public void DeleteEndpointNode(string id)
        {
        }

        public void SaveEndpointNode(Endpoint link)
        {
        }
    }
}