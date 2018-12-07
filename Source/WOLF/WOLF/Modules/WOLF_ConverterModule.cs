using System.Linq;

namespace WOLF
{
    public class WOLF_ConverterModule : WOLF_AbstractPartModule
    {
        public static readonly string INVALID_SITUATION_MESSAGE = "Your vessel must be landed or orbiting in order to connect to a depot.";
        public static readonly string MISSING_DEPOT_MESSAGE = "You must establish a depot in this biome first!";
        public static readonly string MISSING_RESOURCE_MESSAGE = "Depot needs an additional ({0}) {1}.";
        public static readonly string SUCCESSFUL_DEPLOYMENT_MESSAGE = "Your infrastructure has expanded on {0}!";

        protected override void ConnectToDepot()
        {
            // TODO - Might need to add a check for a DepotModule and run its ConnectToDepot method instead?
            // TODO - Might also need to add a check for a HopperModule and display a message saying the hopper needs to be disconnected first.

            var body = vessel.mainBody.name;
            var biome = GetVesselBiome();

            if (biome == string.Empty)
            {
                DisplayMessage(INVALID_SITUATION_MESSAGE);
                return;
            }
            if (!_depotRegistry.HasDepot(body, biome))
            {
                DisplayMessage(MISSING_DEPOT_MESSAGE);
                return;
            }
            if (!Poof.CanGoPoof(vessel))
            {
                DisplayMessage("Your crew must disembark before this vessel can be connected to a depot.");
                return;
            }

            // Get recipes from all attached WOLF PartModules
            var recipes = vessel
                .FindPartModulesImplementing<WOLF_AbstractPartModule>()
                .Select(p => p.Recipe)
                .ToList();

            // Negotiate recipes with the depot
            var depot = _depotRegistry.GetDepot(body, biome);
            var result = depot.Negotiate(recipes);

            if (result is FailedNegotiationResult)
            {
                var failureResult = result as FailedNegotiationResult;
                foreach (var missingResource in failureResult.MissingResources)
                {
                    DisplayMessage(string.Format(MISSING_RESOURCE_MESSAGE, missingResource.Value, missingResource.Key));
                }
                return;
            }

            DisplayMessage(string.Format(SUCCESSFUL_DEPLOYMENT_MESSAGE, body));
            Poof.GoPoof(vessel);
            // TODO - Backup crew roster
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Events["ConnectToDepotEvent"].guiName = "Connect to depot";
        }
    }
}
