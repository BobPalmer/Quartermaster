using System.Collections.Generic;
using System.Linq;

namespace WOLF
{
    public class WOLF_DepotModule : WOLF_AbstractPartModule
    {
        private static readonly string CANNOT_ADD_CREW_MESSAGE = "Kerbals cannot live at this depot yet.";
        private static readonly string DEPOT_ALREADY_ESTABLISHED_MESSAGE = "A depot has already been established here!";
        private static readonly string INVALID_SITUATION_MESSAGE = "Can only estabish a depot when landed on the surface or in orbit.";
        private static readonly string SUCCESSFUL_DEPLOYMENT_MESSAGE = "Your depot has been established at {0} on {1}!";

        public static readonly string HARVESTABLE_RESOURCE_SUFFIX = "Vein";

        [KSPField]
        public string HarvestableResources = string.Empty;

        protected Dictionary<string,int> CalculateAbundance(List<string> resources, HarvestTypes harvestType)
        {
            var abReq = new AbundanceRequest
            {
                Altitude = vessel.altitude,
                BodyId = FlightGlobals.currentMainBody.flightGlobalsIndex,
                CheckForLock = false,
                Latitude = vessel.latitude,
                Longitude = vessel.longitude,
                ResourceType = HarvestTypes.Planetary
            };
            return ResourceManager.GetResourceAbundance(resources, abReq);
        }

        protected Dictionary<string, int> CalculateAbundance(List<string> resources)
        {
            return CalculateAbundance(resources, HarvestTypes.Planetary);
        }

        protected override void ConnectToDepot()
        {
            // Check for issues that would prevent deployment
            var body = vessel.mainBody.name;
            var biome = GetVesselBiome();

            if (biome == string.Empty)
            {
                DisplayMessage(INVALID_SITUATION_MESSAGE);
                return;
            }
            if (_registry.HasDepot(body, biome))
            {
                DisplayMessage(DEPOT_ALREADY_ESTABLISHED_MESSAGE);
                return;
            }
            var otherWolfPartModules = vessel
                .FindPartModulesImplementing<WOLF_AbstractPartModule>()
                .Where(p => p != this);
            var otherWolfHoppers = vessel.FindPartModulesImplementing<WOLF_HopperModule>();
            if (otherWolfPartModules.Any() || otherWolfHoppers.Any())
            {
                DisplayMessage(Messenger.INVALID_DEPOT_PART_ATTACHMENT_MESSAGE);
                return;
            }
            var crew = vessel.GetVesselCrew();
            if (crew != null && crew.Count > 0)
            {
                DisplayMessage(CANNOT_ADD_CREW_MESSAGE);
                return;
            }

            // Establish depot
            var depot = _registry.CreateDepot(body, biome);

            // Calculate resource abundance and cache resource vein names in scenario module
            var harvestableResources = CalculateAbundance(ParseHarvestableResources());
            var knownResources = WOLF_ScenarioModule.AuxillaryResources;
            foreach (var resource in harvestableResources)
            {
                if (!knownResources.Contains(resource.Key))
                {
                    knownResources.Add(resource.Key);
                }
            }

            // Setup starting resource streams
            depot.NegotiateProvider(Recipe.OutputIngredients);
            depot.NegotiateProvider(harvestableResources);
            depot.NegotiateConsumer(Recipe.InputIngredients);

            DisplayMessage(string.Format(SUCCESSFUL_DEPLOYMENT_MESSAGE, biome, body));
            Poof.GoPoof(vessel);
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Events["ConnectToDepotEvent"].guiName = "Establish Depot";
        }

        protected List<string> ParseHarvestableResources()
        {
            return HarvestableResources
                .Split(',')
                .Distinct()
                .ToList();
        }
    }
}
