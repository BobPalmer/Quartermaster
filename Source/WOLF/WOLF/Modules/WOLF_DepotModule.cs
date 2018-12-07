using System.Collections.Generic;

namespace WOLF
{
    public class WOLF_DepotModule : WOLF_AbstractPartModule
    {
        private static readonly string DEPOT_ALREADY_ESTABLISHED_MESSAGE = "A depot has already been established here!";
        private static readonly string INVALID_SITUATION_MESSAGE = "Can only estabish a depot when landed on the surface or in orbit.";
        private static readonly string MISSING_CREW_MESSAGE = "Need {0} more Kerbal(s) with {1} trait.";
        private static readonly string SUCCESSFUL_DEPLOYMENT_MESSAGE = "Your depot has been established at {0} on {1}!";

        protected override void ConnectToDepot()
        {
            // Check for issues that would prevent deployment
            var missingCrew = CheckForMissingCrew();
            if (missingCrew.Count > 0)
            {
                foreach (var missingRequirement in missingCrew)
                {
                    DisplayMessage(string.Format(MISSING_CREW_MESSAGE, missingRequirement.Value, missingRequirement.Key));
                }
                return;
            }

            var body = vessel.mainBody.name;
            var biome = GetVesselBiome();

            if (biome == string.Empty)
            {
                DisplayMessage(INVALID_SITUATION_MESSAGE);
                return;
            }

            if (_depotRegistry.HasDepot(body, biome))
            {
                DisplayMessage(DEPOT_ALREADY_ESTABLISHED_MESSAGE);
                return;
            }

            // Establish depot
            var depot = _depotRegistry.AddDepot(body, biome);

            // Setup starting resource streams
            depot.NegotiateProvider(Recipe.OutputIngredients);
            depot.NegotiateConsumer(Recipe.InputIngredients);

            // TODO - Setup recipes for other attached WOLF PartModules
            // TODO - Backup crew roster
            // TODO - Make vessel go poof (or replace with a single-part vessel that can be expanded with various hoppers for resource/crew retrieval?)

            DisplayMessage(string.Format(SUCCESSFUL_DEPLOYMENT_MESSAGE, biome, body));
        }

        private Dictionary<string, int> CheckForMissingCrew()
        {
            return new Dictionary<string, int>();
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Events["ConnectToDepotEvent"].guiName = "Establish Depot";
        }
    }
}
