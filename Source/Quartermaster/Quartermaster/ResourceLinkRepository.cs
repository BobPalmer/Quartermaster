using System;
using System.Collections.Generic;

namespace Quartermaster
{
    public class NetworkRepository
    {
        //Backing variables
        private List<ResourceLink> _networkLinks;
        private IResourceLinkPersister _persister;

        public NetworkRepository(IResourceLinkPersister persister)
        {
            _persister = persister;
        }

        public void ResetCache()
        {
            _networkLinks = null;
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

        public int RecordCount()
        {
            return NetworkLinks.Count;
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