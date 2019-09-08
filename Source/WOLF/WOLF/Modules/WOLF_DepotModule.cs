﻿using KSP.Localization;
using System.Collections.Generic;
using System.Linq;
using static Vessel;

namespace WOLF
{
    [KSPModule("Depot")]
    public class WOLF_DepotModule : WOLF_AbstractPartModule
    {
        private static string CANNOT_ADD_CREW_MESSAGE = "#autoLOC_USI_WOLF_DEPOT_CANNOT_ADD_CREW_MESSAGE"; // "Kerbals cannot live at this depot yet.";
        private static string DEPOT_ALREADY_ESTABLISHED_MESSAGE = "#autoLOC_USI_WOLF_DEPOT_ALREADY_ESTABLISHED_MESSAGE"; // "A depot has already been established here!";
        private static string ESTABLISH_DEPOT_GUI_NAME = "#autoLOC_USI_WOLF_ESTABLISH_DEPOT_GUI_NAME"; // "Establish depot."
        private static string INVALID_SITUATION_MESSAGE = "#autoLOC_USI_WOLF_DEPOT_INVALID_SITUATION_MESSAGE"; // "Can only estabish a depot when landed on the surface or in orbit.";
        private static string SUCCESSFUL_DEPLOYMENT_MESSAGE = "#autoLOC_USI_WOLF_DEPOT_SUCCESSFUL_DEPLOYMENT_MESSAGE"; // "Your depot has been established at {0} on {1}!";
        private static string SUCCESSFUL_SURVEY_MESSAGE = "#autoLOC_USI_WOLF_DEPOT_SUCCESSFUL_SURVEY_MESSAGE"; // "Survey completed at {0} on {1}!";
        private static string SURVEY_ALREADY_COMPLETED_MESSAGE = "#autoLOC_USI_WOLF_DEPOT_SURVEY_ALREADY_COMPLETE_MESSAGE"; // "A survey has already been completed in this biome!";

        private static readonly HarvestTypes[] DEFAULT_HARVEST_TYPES = new HarvestTypes[] { HarvestTypes.Atmospheric, HarvestTypes.Planetary };
        private static readonly HarvestTypes[] OCEANIC_HARVEST_TYPES = new HarvestTypes[] { HarvestTypes.Atmospheric, HarvestTypes.Oceanic, HarvestTypes.Planetary };
        private static readonly HarvestTypes[] ORBITAL_HARVEST_TYPES = new HarvestTypes[] { HarvestTypes.Exospheric };
        private static readonly Dictionary<string, int> HOME_PLANET_STARTING_RESOURCES = new Dictionary<string, int>
        {
            { "Food", 1 },
            { "MaterialKits", 5 },
            { "Oxygen", 1 },
            { "Power", 10 },
            { "Water", 5 }
        };
        private static readonly Dictionary<string, int> DEFAULT_STARTING_RESOURCES = new Dictionary<string, int>
        {
            { "Power", 5 }
        };

        public const string HARVESTABLE_RESOURCE_SUFFIX = "Vein";

        protected Dictionary<string, int> CalculateAbundance()
        {
            vessel.checkLanded();
            vessel.checkSplashed();

            HarvestTypes[] harvestTypes;
            if (vessel.Splashed)
                harvestTypes = OCEANIC_HARVEST_TYPES;
            else if (vessel.situation == Situations.ORBITING)
                harvestTypes = ORBITAL_HARVEST_TYPES;
            else
                harvestTypes = DEFAULT_HARVEST_TYPES;

            return CalculateAbundance(harvestTypes);
        }

        protected Dictionary<string,int> CalculateAbundance(HarvestTypes[] harvestTypes)
        {
            return ResourceManager.GetResourceAbundance(
                bodyIndex: FlightGlobals.currentMainBody.flightGlobalsIndex,
                altitude: vessel.altitude,
                latitude: vessel.latitude,
                longitude: vessel.longitude,
                harvestTypes: harvestTypes,
                config: _scenario.Configuration);
        }

        protected override void ConnectToDepot()
        {
            EstablishDepot(false);
        }

        protected void EstablishDepot(bool isSurvey)
        {
            // Check for issues that would prevent deployment
            var body = vessel.mainBody.name;

            var biome = GetVesselBiome();
            if (biome == string.Empty)
            {
                DisplayMessage(INVALID_SITUATION_MESSAGE);
                return;
            }

            bool depotAlreadyExists = _registry.HasDepot(body, biome);
            IDepot depot = null;
            if (depotAlreadyExists)
                depot = _registry.GetDepot(body, biome);

            if (isSurvey)
            {
                if (depotAlreadyExists && depot.IsSurveyed)
                {
                    DisplayMessage(SURVEY_ALREADY_COMPLETED_MESSAGE);
                    return;
                }
            }
            else
            {
                if (depotAlreadyExists && depot.IsEstablished)
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
            }

            // Create depot if necessary
            if (!depotAlreadyExists)
            {
                depot = _registry.CreateDepot(body, biome);
            }

            if (isSurvey)
            {
                // Survey biome
                depot.Survey();

                // Calculate resource abundance and cache resource vein names in scenario module
                var harvestableResources = CalculateAbundance();
                depot.NegotiateProvider(harvestableResources);

                DisplayMessage(string.Format(SUCCESSFUL_SURVEY_MESSAGE, biome, body));
            }
            else
            {
                // Establish depot
                depot.Establish();

                // Setup bootstrap resource streams
                if (vessel.mainBody.isHomeWorld && vessel.situation != Situations.ORBITING)
                {
                    depot.NegotiateProvider(HOME_PLANET_STARTING_RESOURCES);
                }
                else
                {
                    depot.NegotiateProvider(DEFAULT_STARTING_RESOURCES);
                }
                depot.NegotiateProvider(WolfRecipe.OutputIngredients);
                depot.NegotiateConsumer(WolfRecipe.InputIngredients);

                DisplayMessage(string.Format(SUCCESSFUL_DEPLOYMENT_MESSAGE, biome, body));
                Poof.GoPoof(vessel);
            }
        }

        public override string GetInfo()
        {
            return PartInfo;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_DEPOT_CANNOT_ADD_CREW_MESSAGE", out string crewMessage))
            {
                CANNOT_ADD_CREW_MESSAGE = crewMessage;
            }
            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_DEPOT_ALREADY_ESTABLISHED_MESSAGE", out string depotMessage))
            {
                DEPOT_ALREADY_ESTABLISHED_MESSAGE = depotMessage;
            }
            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_DEPOT_INVALID_SITUATION_MESSAGE", out string situationMessage))
            {
                INVALID_SITUATION_MESSAGE = situationMessage;
            }
            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_DEPOT_SUCCESSFUL_DEPLOYMENT_MESSAGE", out string deploySuccessMessage))
            {
                SUCCESSFUL_DEPLOYMENT_MESSAGE = deploySuccessMessage;
            }
            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_DEPOT_SUCCESSFUL_SURVEY_MESSAGE", out string surveySuccessMessage))
            {
                SUCCESSFUL_SURVEY_MESSAGE = surveySuccessMessage;
            }
            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_DEPOT_SURVEY_ALREADY_COMPLETE_MESSAGE", out string surveyCompletedMessage))
            {
                SURVEY_ALREADY_COMPLETED_MESSAGE = surveyCompletedMessage;
            }

            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_ESTABLISH_DEPOT_GUI_NAME", out string establishGuiName))
            {
                ESTABLISH_DEPOT_GUI_NAME = establishGuiName;
            }
            Events["ConnectToDepotEvent"].guiName = ESTABLISH_DEPOT_GUI_NAME;
            Actions["ConnectToDepotAction"].guiName = ESTABLISH_DEPOT_GUI_NAME;
        }
    }
}
