using System.Collections.Generic;

namespace Quartermaster
{
    public interface INetworkPersister
    {
        //Links
        List<ResourceLink> GetLinkInfo();
        void DeleteLinkNode(string id);
        void SaveLinkNode(ResourceLink link);
        //Routes
        List<Route> GetRouteInfo();
        void DeleteRouteNode(string id);
        void SaveRouteNode(Route route);
        //Endpoints
        List<Endpoint> GetEndpointInfo();
        void DeleteEndpointNode(string id);
        void SaveEndpointNode(Endpoint link);

    }
}