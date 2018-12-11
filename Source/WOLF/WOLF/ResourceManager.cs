using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WOLF
{
    public static class ResourceManager
    {
        public const int RESOURCE_ABUNDANCE_CEILING = 1000;
        public const int RESOURCE_ABUNDANCE_FLOOR = 1;
        public const int RESOURCE_ABUNCANCE_MULTIPLIER = 100;

        public static Dictionary<string, int> GetResourceAbundance(List<string> resources, AbundanceRequest abReq)
        {
            var resourceList = new Dictionary<string, int>();
            foreach(var resource in resources)
            {
                abReq.ResourceName = resource;
                var baseAbundance = ResourceMap.Instance.GetAbundance(abReq);
                int abundance = (int)(baseAbundance * (double)RESOURCE_ABUNCANCE_MULTIPLIER);
                if(abundance > RESOURCE_ABUNDANCE_FLOOR)
                {
                    abundance = Math.Min(abundance, RESOURCE_ABUNDANCE_CEILING);
                }
                else
                {
                    abundance = 0;
                }
                resourceList.Add(resource, abundance);
            }
            return resourceList;
        }
    }
}
