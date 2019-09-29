using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WOLF
{
    public class ConfigurationFromFile
    {
        public string AllowedHarvestableResources { get; set; }
        public string BlacklistedHomeworldResources { get; set; }
    }

    public class Configuration
    {
        private static readonly Regex _sanitizeRegex = new Regex(@"\s+");

        public static readonly List<string> DefaultHarvestableResources = new List<string>
        {
            "Dirt", "ExoticMinerals", "Gypsum", "Hydrates", "MetallicOre", "Minerals",
            "Ore", "Oxygen", "RareMetals", "Silicates", "Substrate", "Water", "XenonGas"
        };

        private List<string> _allowedHarvestableResources;
        private List<string> _allowedHarvestableResourcesOnHomeworld;
        private List<string> _blacklistedHomeworldResources;

        public List<string> AllowedHarvestableResources
        {
            get
            {
                if (_allowedHarvestableResources == null || _allowedHarvestableResources.Count < 1)
                {
                    _allowedHarvestableResources = DefaultHarvestableResources;
                }

                return _allowedHarvestableResources;
            }
        }

        public List<string> AllowedHarvestableResourcesOnHomeworld
        {
            get
            {
                if (_allowedHarvestableResourcesOnHomeworld == null)
                {
                    _allowedHarvestableResourcesOnHomeworld = AllowedHarvestableResources
                        .Where(r => !_blacklistedHomeworldResources.Contains(r))
                        .ToList();
                }

                return _allowedHarvestableResourcesOnHomeworld;
            }
        }

        public static List<string> ParseHarvestableResources(string resources)
        {
            if (string.IsNullOrEmpty(resources))
            {
                return new List<string>();
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

        public void SetBlacklistedHomeworldResources(List<string> resources)
        {
            if (resources == null)
            {
                _blacklistedHomeworldResources = new List<string>();
            }
            else
            {
                _blacklistedHomeworldResources = resources
                    .Distinct()
                    .OrderBy(r => r)
                    .ToList();
            }
        }

        public void SetBlacklistedHomeworldResources(string resourceList)
        {
            _blacklistedHomeworldResources = ParseHarvestableResources(resourceList);
        }

        public void SetHarvestableResources(List<string> resources)
        {
            if (resources != null)
            {
                _allowedHarvestableResources = resources
                    .Distinct()
                    .OrderBy(r => r)
                    .ToList();
            }
        }

        public void SetHarvestableResources(string resourceList)
        {
            _allowedHarvestableResources = ParseHarvestableResources(resourceList);
        }
    }
}
