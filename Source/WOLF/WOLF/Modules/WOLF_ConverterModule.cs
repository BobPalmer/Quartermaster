﻿using KSP.Localization;
using System.Linq;

namespace WOLF
{
    public class WOLF_ConverterModule : WOLF_AbstractPartModule
    {
        protected static string CONNECT_TO_DEPOT_GUI_NAME = "#autoLOC_USI_WOLF_CONNECT_TO_DEPOT_GUI_NAME"; // "Connect to depot.";
        protected static string CREW_NOT_ELIGIBLE_MESSAGE = "#autoLOC_USI_WOLF_CREW_NOT_ELIGIBLE_MESSAGE"; // "Kerbals need some experience before they can work in a colony.";

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
            var otherDepotModules = vessel.FindPartModulesImplementing<WOLF_DepotModule>()
                .Where(p => !(p is WOLF_SurveyModule));
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
                .Where(p => !(p is WOLF_SurveyModule))
                .Select(p => p.WolfRecipe)
                .ToList();

            // Add crew recipe
            var crewModule = vessel.vesselModules
                .Where(m => m is WOLF_CrewModule)
                .FirstOrDefault() as WOLF_CrewModule;
            if (crewModule == null)
            {
                DisplayMessage("BUG: Could not find crew module.");
                return;
            }
            else if (!crewModule.IsCrewEligible())
            {
                DisplayMessage(CREW_NOT_ELIGIBLE_MESSAGE);
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

            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_CREW_NOT_ELIGIBLE_MESSAGE", out string crewNotEligibleMessage))
            {
                CREW_NOT_ELIGIBLE_MESSAGE = crewNotEligibleMessage;
            }

            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_CONNECT_TO_DEPOT_GUI_NAME", out string connectGuiName))
            {
                CONNECT_TO_DEPOT_GUI_NAME = connectGuiName;
            }
            Events["ConnectToDepotEvent"].guiName = CONNECT_TO_DEPOT_GUI_NAME;
            Actions["ConnectToDepotAction"].guiName = CONNECT_TO_DEPOT_GUI_NAME;
        }
    }
}
