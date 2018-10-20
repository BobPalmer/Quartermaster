using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quartermaster
{
    public class NetworkPersistence : MonoBehaviour, INetworkPersister
    {
        public ConfigNode ScenarioNode { get; private set; }
        private List<ResourceLink> _linkInfo;
        private List<Route> _routeInfo;
        private List<Endpoint> _endpointInfo;

        public void Load(ConfigNode node)
        {
            if (node.HasNode("QUARTERMASTER_SETTINGS"))
            {
                ScenarioNode = node.GetNode("QUARTERMASTER_SETTINGS");
                _linkInfo = SetupLinkInfo();
                _routeInfo = SetupRouteInfo();
                _endpointInfo = SetupEndpointInfo();
                //Reset cache
                ResourceNetwork.Instance.Repo.ResetCache();
            }
            else
            {
                _linkInfo = new List<ResourceLink>();
                _routeInfo = new List<Route	>();
                _endpointInfo = new List<Endpoint	>();
            }
        }

        public bool isLoaded()
        {
            return ScenarioNode != null;
        }
        private List<ResourceLink> SetupLinkInfo()
        {
            print("Loading Link Nodes");
            ConfigNode[] linkNodes = ScenarioNode.GetNodes("LINK_DATA");
            print("LinkNodeCount:  " + linkNodes.Length);
            return ImportLinkNodeList(linkNodes);
        }

        public List<ResourceLink> GetLinkInfo()
        {
            return _linkInfo ?? (_linkInfo = SetupLinkInfo());
        }

        private List<Route> SetupRouteInfo()
        {
            print("Loading Routes");
            ConfigNode[] routes = ScenarioNode.GetNodes("ROUTE_DATA");
            print("Route Count:  " + routes.Length);
            return ImportRouteNodeList(routes);
        }

        public List<Route> GetRouteInfo()
        {
            return _routeInfo ?? (_routeInfo = SetupRouteInfo());
        }

        private List<Endpoint> SetupEndpointInfo()
        {
            print("Loading Endpoints");
            ConfigNode[] epNodes = ScenarioNode.GetNodes("ENDPOINT_DATA");
            print("Endpoint Count:  " + epNodes.Length);
            return ImportEndpointNodeList(epNodes);
        }

        public List<Endpoint> GetEndpointInfo()
        {
            return _endpointInfo ?? (_endpointInfo = SetupEndpointInfo());
        }
        
        public void Save(ConfigNode node)
        {
            if (node.HasNode("QUARTERMASTER_SETTINGS"))
            {
                ScenarioNode = node.GetNode("QUARTERMASTER_SETTINGS");
            }
            else
            {
                ScenarioNode = node.AddNode("QUARTERMASTER_SETTINGS");
            }

            if (_linkInfo != null)
            {
                var count = _linkInfo.Count;
                for (int i = 0; i < count; ++i)
                {
                    var r = _linkInfo[i];
                    var rNode = new ConfigNode("LINK_DATA");
                    rNode.AddValue("LinkId", r.LinkId);
                    rNode.AddValue("SourceId", r.SourceId);
                    rNode.AddValue("DestinationId", r.DestinationId);
                    rNode.AddValue("ResourceName", r.ResourceName);
                    rNode.AddValue("Quantity", r.Quantity);
                    ScenarioNode.AddNode(rNode);
                }
            }
            if (_routeInfo	!= null)
            {
                var count = _routeInfo.Count;
                for (int i = 0; i < count; ++i)
                {
                    var r = _routeInfo[i];
                    var rNode = new ConfigNode("ROUTE_DATA");
                    rNode.AddValue("LinkId", r.LinkId);
                    rNode.AddValue("RouteId", r.RouteId);
                    rNode.AddValue("EndpointId", r.EndpointId);
                    ScenarioNode.AddNode(rNode);
                }
            }
            if (_endpointInfo != null)
            {
                var count = _endpointInfo.Count;
                for (int i = 0; i < count; ++i)
                {
                    var r = _endpointInfo[i];
                    var rNode = new ConfigNode("ENDPOINT_DATA");
                    rNode.AddValue("EndpointId", r.EndpointId);
                    rNode.AddValue("VesselId", r.VesselId);
                    rNode.AddValue("MainBodyIndex", r.MainBodyIndex);
                    rNode.AddValue("IsLanded", r.IsLanded);
                    rNode.AddValue("Type", (int)r.Type);
                    ScenarioNode.AddNode(rNode);
                }
            }

            //Reset cache
            ResourceNetwork.Instance.Repo.ResetCache();
        }

        public static int GetValue(ConfigNode config, string name, int currentValue)
        {
            int newValue;
            if (config.HasValue(name) && int.TryParse(config.GetValue(name), out newValue))
            {
                return newValue;
            }
            return currentValue;
        }

        public static bool GetValue(ConfigNode config, string name, bool currentValue)
        {
            bool newValue;
            if (config.HasValue(name) && bool.TryParse(config.GetValue(name), out newValue))
            {
                return newValue;
            }
            return currentValue;
        }

        public static float GetValue(ConfigNode config, string name, float currentValue)
        {
            float newValue;
            if (config.HasValue(name) && float.TryParse(config.GetValue(name), out newValue))
            {
                return newValue;
            }
            return currentValue;
        }

        public string AddLinkNode(ResourceLink res)
        {
            if (String.IsNullOrEmpty(res.LinkId))
            {
                Guid id = Guid.NewGuid();
                res.LinkId = id.ToString();
            }
            _linkInfo.Add(res);
            return res.LinkId;
        }

        public void DeleteLinkNode(string id)
        {
            var count = _linkInfo.Count;
            for (int i = 0; i < count; ++i)
            {
                var k = _linkInfo[i];
                if (k.LinkId == id)
                {
                    _linkInfo.Remove(k);
                    return;
                }
            }
        }

        public static List<ResourceLink> ImportLinkNodeList(ConfigNode[] nodes)
        {
            var nList = new List<ResourceLink>();
            var count = nodes.Length;
            for (int i = 0; i < count; ++i)
            {
                var node = nodes[i];
                var res = ResourceUtilities.LoadNodeProperties<ResourceLink>(node);
                nList.Add(res);
            }
            return nList;
        }
 
        public void SaveLinkNode(ResourceLink saveLink)
        {
            ResourceLink newLink = null;
            var count = _linkInfo.Count;
            for (int i = 0; i < count; ++i)
            {
                var n = _linkInfo[i];
                if (n.LinkId == saveLink.LinkId)
                {
                    newLink = n;
                    break;
                }
            }

            if (newLink == null)
            {
                newLink = new ResourceLink();
                newLink.LinkId = saveLink.LinkId;
                _linkInfo.Add(newLink);
            }
            newLink.LinkId = saveLink.LinkId;
            newLink.SourceId = saveLink.SourceId;
            newLink.DestinationId = saveLink.DestinationId;
            newLink.ResourceName = saveLink.ResourceName;
            newLink.Quantity = saveLink.Quantity;
        }


        public string AddRouteNode(Route route)
        {
            if (String.IsNullOrEmpty(route.RouteId))
            {
                Guid id = Guid.NewGuid();
                route.RouteId = id.ToString();
            }
            _routeInfo.Add(route);
            return route.RouteId;
        }

        public void DeleteRouteNode(string id)
        {
            var count = _routeInfo.Count;
            for (int i = 0; i < count; ++i)
            {
                var k = _routeInfo[i];
                if (k.RouteId == id)
                {
                    _routeInfo.Remove(k);
                    return;
                }
            }
        }

        public static List<Route> ImportRouteNodeList(ConfigNode[] nodes)
        {
            var nList = new List<Route>();
            var count = nodes.Length;
            for (int i = 0; i < count; ++i)
            {
                var node = nodes[i];
                var route = ResourceUtilities.LoadNodeProperties<Route>(node);
                nList.Add(route);
            }
            return nList;
        }

        public void SaveRouteNode(Route saveRoute)
        {
            Route newRoute = null;
            var count = _routeInfo.Count;
            for (int i = 0; i < count; ++i)
            {
                var n = _routeInfo[i];
                if (n.LinkId == saveRoute.LinkId)
                {
                    newRoute = n;
                    break;
                }
            }

            if (newRoute == null)
            {
                newRoute = new Route();
                newRoute.LinkId = saveRoute.LinkId;
                _routeInfo.Add(newRoute);
            }
            newRoute.LinkId = saveRoute.LinkId;
            newRoute.RouteId = saveRoute.RouteId;
            newRoute.EndpointId = saveRoute.EndpointId;
        }


        public void AddEndpointNode(Endpoint ep)
        {
            _endpointInfo.Add(ep);
        }

        public void DeleteEndpointNode(string id)
        {
            var count = _endpointInfo.Count;
            for (int i = 0; i < count; ++i)
            {
                var k = _endpointInfo[i];
                if (k.EndpointId == id)
                {
                    _endpointInfo.Remove(k);
                    return;
                }
            }
        }

        public static List<Endpoint> ImportEndpointNodeList(ConfigNode[] nodes)
        {
            var nList = new List<Endpoint>();
            var count = nodes.Length;
            for (int i = 0; i < count; ++i)
            {
                var node = nodes[i];
                var res = ResourceUtilities.LoadNodeProperties<Endpoint>(node);
                nList.Add(res);
            }
            return nList;
        }

        public void SaveEndpointNode(Endpoint saveLink)
        {
            Endpoint newEp = null;
            var count = _endpointInfo.Count;
            for (int i = 0; i < count; ++i)
            {
                var n = _endpointInfo[i];
                if (n.EndpointId == saveLink.EndpointId)
                {
                    newEp = n;
                    break;
                }
            }

            if (newEp == null)
            {
                newEp = new Endpoint	();
                newEp.EndpointId = saveLink.EndpointId;
                _endpointInfo.Add(newEp);
            }
            newEp.EndpointId = saveLink.EndpointId;
            newEp.VesselId = saveLink.VesselId;
            newEp.Type = saveLink.Type;
            newEp.IsLanded = saveLink.IsLanded;
            newEp.MainBodyIndex = saveLink.MainBodyIndex;
        }
    }
}