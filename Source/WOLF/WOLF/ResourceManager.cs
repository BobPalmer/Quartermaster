using System;
using System.Collections.Generic;
using System.Linq;

namespace WOLF
{
    public static class ResourceManager
    {
        public const int RESOURCE_ABUNDANCE_CEILING = 1000;
        public const int RESOURCE_ABUNDANCE_FLOOR = 1;
        public const double RESOURCE_ABUNDANCE_MULTIPLIER = 100d;
        public const double RESOURCE_ABUNDANCE_RADIUS_MULT = 250d;

        public static Dictionary<string, int> GetResourceAbundance(
            int bodyIndex,
            double altitude,
            double latitude,
            double longitude,
            HarvestTypes[] harvestTypes,
            Configuration config)
        {
            if (config == null)
            {
                config = new Configuration();
                config.SetHarvestableResources("");
            }
            else if (config.AllowedHarvestableResources == null
                || config.AllowedHarvestableResources.Count < 1)
            {
                config.SetHarvestableResources("");
            }

            var abundanceRequest = new AbundanceRequest
            {
                Altitude = altitude,
                BodyId = bodyIndex,
                CheckForLock = false,
                Latitude = latitude,
                Longitude = longitude
            };
            var radiusMultiplier = Math.Sqrt(FlightGlobals.Bodies[bodyIndex].Radius) / RESOURCE_ABUNDANCE_RADIUS_MULT;
            var resourceList = new Dictionary<string, int>();
            foreach (var harvestType in harvestTypes.Distinct())
            {
                abundanceRequest.ResourceType = harvestType;

                var harvestableResources = ResourceMap.Instance.FetchAllResourceNames(harvestType);
                foreach (var resource in harvestableResources)
                {
                    abundanceRequest.ResourceName = resource;

                    var baseAbundance = ResourceMap.Instance.GetAbundance(abundanceRequest);
                    int abundance = (int)(baseAbundance * RESOURCE_ABUNDANCE_MULTIPLIER * radiusMultiplier);
                    if (abundance > RESOURCE_ABUNDANCE_FLOOR)
                    {
                        abundance = Math.Min(abundance, RESOURCE_ABUNDANCE_CEILING);
                    }
                    else
                    {
                        abundance = 0;
                    }

                    if (config.AllowedHarvestableResources.Contains(resource))
                    {
                        var wolfResourceName = resource + WOLF_DepotModule.HARVESTABLE_RESOURCE_SUFFIX;
                        if (resourceList.ContainsKey(wolfResourceName))
                        {
                            resourceList[wolfResourceName] += abundance;
                        }
                        else
                        {
                            resourceList.Add(wolfResourceName, abundance);
                        }
                    }
                }
            }

            return resourceList;
        }
    }
}
