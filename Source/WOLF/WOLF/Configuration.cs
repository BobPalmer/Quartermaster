using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WOLF
{
    public class ConfigurationFromFile
    {
        public string AllowedHarvestableResources { get; set; }
    }

    public class Configuration
    {
        private static readonly Regex _sanitizeRegex = new Regex(@"\s+");

        public static readonly List<string> DefaultHarvestableResources = new List<string>
        {
            "Dirt", "ExoticMinerals", "Gypsum", "Hydrates", "MetallicOre", "Minerals",
            "Ore", "Oxygen", "RareMetals", "Silicates", "Substrate", "Water"
        };

        public List<string> AllowedHarvestableResources { get; set; }

        public static List<string> ParseHarvestableResources(string resources)
        {
            if (string.IsNullOrEmpty(resources))
            {
                return DefaultHarvestableResources;
            }
            else
            {
                var sanitizedList = _sanitizeRegex.Replace(resources, string.Empty);
                var tokens = sanitizedList.Split(',');
                return tokens
                    .Where(t => !string.IsNullOrEmpty(t))
                    .Distinct()
                    .OrderBy(t => t)
                    .ToList();
            }
        }

        public void SetHarvestableResources(List<string> resources)
        {
            if (resources == null || resources.Count < 1)
            {
                AllowedHarvestableResources = DefaultHarvestableResources;
            }
            else
            {
                AllowedHarvestableResources = resources
                    .Distinct()
                    .OrderBy(r => r)
                    .ToList();
            }
        }

        public void SetHarvestableResources(string resourceList)
        {
            AllowedHarvestableResources = ParseHarvestableResources(resourceList);
        }
    }
}
