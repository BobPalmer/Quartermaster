using System.Linq;

namespace WOLF
{
    public class WOLF_ConverterModule : WOLF_AbstractPartModule
    {
        protected override void ConnectToDepot()
        {
            // Check for issues that would prevent deployment
            var body = vessel.mainBody.name;
            var biome = GetVesselBiome();

            if (biome == string.Empty)
            {
                DisplayMessage(Messenger.INVALID_SITUATION_MESSAGE);
                return;
            }
            if (!_depotRegistry.HasDepot(body, biome))
            {
                DisplayMessage(Messenger.MISSING_DEPOT_MESSAGE);
                return;
            }
            var otherDepotModules = vessel.FindPartModulesImplementing<WOLF_DepotModule>();
            if (otherDepotModules.Any())
            {
                DisplayMessage(Messenger.INVALID_DEPOT_PART_ATTACHMENT_MESSAGE);
                return;
            }
            var otherHopperModules = vessel.FindPartModulesImplementing<WOLF_HopperModule>();
            if (otherHopperModules.Any())
            {
                DisplayMessage(Messenger.INVALID_HOPPER_PART_ATTACHMENT_MESSAGE);
                return;
            }

            // Get recipes from all attached WOLF PartModules
            var recipes = vessel
                .FindPartModulesImplementing<WOLF_AbstractPartModule>()
                .Select(p => p.Recipe)
                .ToList();

            // Add crew recipe
            var crewModule = vessel.vesselModules
                .Where(m => m is WOLF_CrewModule)
                .FirstOrDefault() as WOLF_CrewModule;
            if (crewModule == null)
            {
                DisplayMessage("Could not find crew module.");
                return;
            }
            else
            {
                var crewRecipe = crewModule.GetCrewRecipe();
                recipes.Add(crewRecipe);
            }

            // Negotiate recipes with the depot
            var depot = _depotRegistry.GetDepot(body, biome);
            var result = depot.Negotiate(recipes);

            if (result is FailedNegotiationResult)
            {
                var failureResult = result as FailedNegotiationResult;
                foreach (var missingResource in failureResult.MissingResources)
                {
                    DisplayMessage(string.Format(Messenger.MISSING_RESOURCE_MESSAGE, missingResource.Value, missingResource.Key));
                }
                return;
            }

            DisplayMessage(string.Format(Messenger.SUCCESSFUL_DEPLOYMENT_MESSAGE, body));
            Poof.GoPoof(vessel);
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Events["ConnectToDepotEvent"].guiName = "Connect to depot";
        }
    }
}
