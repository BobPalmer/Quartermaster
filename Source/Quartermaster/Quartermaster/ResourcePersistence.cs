using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quartermaster
{
    public class ResourcePersistence : MonoBehaviour, IResourceLinkPersister
    {
        public ConfigNode ScenarioNode { get; private set; }
        private List<ResourceLink> _linkInfo;

        public void Load(ConfigNode node)
        {
            if (node.HasNode("QUARTERMASTER_SETTINGS"))
            {
                ScenarioNode = node.GetNode("QUARTERMASTER_SETTINGS");
                _linkInfo = SetupLinkInfo();
                //Reset cache
                ResourceNetwork.Instance.Repo.ResetCache();
            }
            else
            {
                _linkInfo = new List<ResourceLink>();
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
                    rNode.AddValue("Id", r.LinkId);
                    rNode.AddValue("Source", r.SourceId);
                    rNode.AddValue("Destination", r.DestinationId);
                    rNode.AddValue("ResourceName", r.ResourceName);
                    rNode.AddValue("Quantity", r.Quantity);
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
    }
}