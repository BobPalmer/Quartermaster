using System.Linq;

namespace WOLF
{
    public class WOLF_ConverterModule : WOLF_AbstractPartModule
    {
        /// <summary>
        /// Checks for issues that would prevent connecting to a depot.
        /// </summary>
        /// <returns>A message if there was an error, otherwise empty string.</returns>
        protected string CanConnectToDepot()
        {
            var body = vessel.mainBody.name;
            var biome = GetVesselBiome();

            if (biome == string.Empty)
            {
                return Messenger.INVALID_SITUATION_MESSAGE;
            }
            if (!_registry.HasDepot(body, biome))
            {
                return Messenger.MISSING_DEPOT_MESSAGE;
            }
            var otherDepotModules = vessel.FindPartModulesImplementing<WOLF_DepotModule>();
            if (otherDepotModules.Any())
            {
                return Messenger.INVALID_DEPOT_PART_ATTACHMENT_MESSAGE;
            }
            var otherHopperModules = vessel.FindPartModulesImplementing<WOLF_HopperModule>();
            if (otherHopperModules.Any())
            {
                return Messenger.INVALID_HOPPER_PART_ATTACHMENT_MESSAGE;
            }

            return string.Empty;
        }

        protected override void ConnectToDepot()
        {
            // Check for issues that would prevent deployment
            var deployCheckResult = CanConnectToDepot();
            if (!string.IsNullOrEmpty(deployCheckResult))
            {
                DisplayMessage(deployCheckResult);
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
            var body = vessel.mainBody.name;
            var biome = GetVesselBiome();
            var depot = _registry.GetDepot(body, biome);
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
