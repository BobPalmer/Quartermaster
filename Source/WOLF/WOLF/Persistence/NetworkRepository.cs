using System;
using System.Collections.Generic;

namespace Quartermaster
{
    public class NetworkRepository 
    {
        //Backing variables
        private List<ResourceLink> _networkLinks;
        private List<Route> _routes;
        private List<Endpoint> _endpoints;
        private INetworkPersister _persister;

        public NetworkRepository(INetworkPersister persister)
        {
            _persister = persister;
        }

        public void ResetCache()
        {
            _networkLinks = null;
            _routes = null;
            _endpoints = null;
        }

        public List<ResourceLink> NetworkLinks
        {
            get
            {
                if (_networkLinks == null)
                {
                    _networkLinks = new List<ResourceLink>();
                    _networkLinks.AddRange(_persister.GetLinkInfo());
                }
                return _networkLinks;
            }
        }

        public List<Route> Routes
        {
            get
            {
                if (_routes	 == null)
                {
                    _routes = new List<Route>();
                    _routes.AddRange(_persister.GetRouteInfo());
                }
                return _routes;
            }
        }

        public List<Endpoint> Endpoints
        {
            get
            {
                if (_endpoints	 == null)
                {
                    _endpoints = new List<Endpoint>();
                    _endpoints.AddRange(_persister.GetEndpointInfo());
                }
                return _endpoints;
            }
        }

        public void DeleteLink(string id)
        {
            var count = NetworkLinks.Count;
            for (int i = count; i --> 0;)
            {
                var link = NetworkLinks[i];
                if (link.LinkId == id)
                {
                    NetworkLinks.Remove(link);
                }
            }
            _persister.DeleteLinkNode(id);
        }

        public ResourceLink FetchLink(string id)
        {
            ResourceLink link = null;
            var count = NetworkLinks.Count;
            for (int i = 0; i < count; ++i)
            {
                if (NetworkLinks[i].LinkId == id)
                {
                    link = NetworkLinks[i];
                    break;
                }
            }
            return link;
        }

        public string SaveLink(ResourceLink link)
        {
            var count = NetworkLinks.Count;
            var id = link.LinkId;
            var isNew = true;

            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
                link.LinkId = id;
            }
            else
            {
                for (int i = 0; i < count; ++i)
                {
                    if (NetworkLinks[i].LinkId == link.LinkId)
                    {
                        isNew = false;
                        break;
                    }
                }
            }
            if (isNew)
            {
                NetworkLinks.Add(link);
            }
            _persister.SaveLinkNode(link);
            return id;
        }


        public void DeleteRoute(string id)
        {
            var count = Routes.Count;
            for (int i = count; i-- > 0;)
            {
                var route = Routes[i];
                if (route.RouteId == id)
                {
                    Routes.Remove(route);
                }
            }
            _persister.DeleteRouteNode(id);
        }

        public Route FetchRoute(string id)
        {
            Route route = null;
            var count = Routes.Count;
            for (int i = 0; i < count; ++i)
            {
                if (Routes[i].RouteId == id)
                {
                    route = Routes[i];
                    break;
                }
            }
            return route;
        }

        public string SaveRout(Route route)
        {
            var count = Routes.Count;
            var id = route.RouteId;
            var isNew = true;

            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
                route.RouteId = id;
            }
            else
            {
                for (int i = 0; i < count; ++i)
                {
                    if (Routes[i].RouteId == route.RouteId)
                    {
                        isNew = false;
                        break;
                    }
                }
            }
            if (isNew)
            {
                Routes.Add(route);
            }
            _persister.SaveRouteNode(route);
            return id;
        }


        public void DeleteEndpoint(string id)
        {
            var count = Endpoints.Count;
            for (int i = count; i-- > 0;)
            {
                var ep = Endpoints[i];
                if (ep.EndpointId == id)
                {
                    Endpoints.Remove(ep);
                }
            }
            _persister.DeleteEndpointNode(id);
        }

        public Endpoint	FetchEndpoint(string id)
        {
            Endpoint ep = null;
            var count = Endpoints.Count;
            for (int i = 0; i < count; ++i)
            {
                if (Endpoints[i].EndpointId == id)
                {
                    ep = Endpoints[i];
                    break;
                }
            }
            return ep;
        }

        public bool EndpointExists(string id)
        {
            var count = Endpoints.Count;
            for (int i = 0; i < count; ++i)
            {
                if (Endpoints[i].EndpointId == id)
                {
                    return true;
                }
            }
            return false;
        }

        public string SaveEndpoint(Endpoint ep)
        {
            var count = Endpoints.Count;
            var id = ep.EndpointId;
            var isNew = true;

            for (int i = 0; i < count; ++i)
            {
                if (Endpoints[i].EndpointId == ep.EndpointId)
                {
                    isNew = false;
                    break;
                }
            }
            if (isNew)
            {
                Endpoints.Add(ep);
            }
            _persister.SaveEndpointNode(ep);
            return id;
        }

        public int LinkCount()
        {
            return NetworkLinks.Count;
        }

        public int RouteCount()
        {
            return Routes.Count;
        }

        public int EndpointCount()
        {
            return Endpoints.Count;
        }

        public int GetResourceQuantity(string poolId, string resourceName)
        {
            var amount = 0;
            var count = NetworkLinks.Count;
            for (int i = 0; i < count; ++i)
            {
                var link = NetworkLinks[i];
                if (link.ResourceName == resourceName)
                {
                    if (link.DestinationId == poolId)
                        amount += link.Quantity;
                    if (link.SourceId == poolId)
                        amount -= link.Quantity;
                }
            }
            return amount;
        }
    }
}