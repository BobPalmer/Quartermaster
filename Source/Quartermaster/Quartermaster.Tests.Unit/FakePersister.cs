using System.Collections.Generic;

namespace Quartermaster.Tests.Unit
{
    public class FakePersister : IResourceLinkPersister
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
            return;
        }

        public void SaveLinkNode(ResourceLink link)
        {
            return;
        }
    }
}