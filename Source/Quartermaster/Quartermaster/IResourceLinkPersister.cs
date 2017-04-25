using System.Collections.Generic;

namespace Quartermaster
{
    public interface IResourceLinkPersister
    {
        List<ResourceLink> GetLinkInfo();
        void DeleteLinkNode(string id);
        void SaveLinkNode(ResourceLink link);
    }
}