using System.Collections.Generic;
using UnityEngine;
using USITools;

using WAPM = WOLF.WOLF_AbstractPartModule;
using WCM = WOLF.WOLF_ConverterModule;

namespace WOLF.Modules
{
    public class WOLF_HopperModule : USI_Converter
    {
        private static readonly string ALREADY_CONNECTED_MESSAGE = "This hopper is already connected to a depot!";
        private static readonly string LOST_CONNECTION_MESSAGE = "This hopper has lost its connection to the depot!";
        private static readonly string NOT_CONNECTED_MESSAGE = "You must connect this hopper to a depot first!";

        private IDepotRegistry _depotRegistry;
        private IRecipe _wolfRecipe;

        [KSPField]
        public string InputResources = "";

        [KSPField(isPersistant = true)]
        private bool IsConnectedToDepot = false;

        [KSPField(isPersistant = true)]
        private string DepotBiome = string.Empty;

        [KSPField(isPersistant = true)]
        private string DepotBody = string.Empty;

        [KSPEvent(guiName = "Connect to depot", active = true, guiActive = true, guiActiveEditor = false)]
        public void ConnectToDepotEvent()
        {
            if (IsConnectedToDepot)
            {
                WAPM.DisplayMessage(ALREADY_CONNECTED_MESSAGE);
                return;
            }

            var body = vessel.mainBody.name;
            var biome = GetVesselBiome();

            if (biome == string.Empty)
            {
                WAPM.DisplayMessage(WCM.INVALID_SITUATION_MESSAGE);
                return;
            }
            if (!_depotRegistry.HasDepot(body, biome))
            {
                WAPM.DisplayMessage(WCM.MISSING_DEPOT_MESSAGE);
                return;
            }

            // Negotiate recipes with the depot
            var depot = _depotRegistry.GetDepot(body, biome);
            var result = depot.Negotiate(_wolfRecipe);

            if (result is FailedNegotiationResult)
            {
                var failureResult = result as FailedNegotiationResult;
                foreach (var missingResource in failureResult.MissingResources)
                {
                    WAPM.DisplayMessage(string.Format(WCM.MISSING_RESOURCE_MESSAGE, missingResource.Value, missingResource.Key));
                }
                return;
            }

            DepotBody = body;
            DepotBiome = biome;
            IsConnectedToDepot = true;

            WAPM.DisplayMessage(string.Format(WCM.SUCCESSFUL_DEPLOYMENT_MESSAGE, body));
        }

        protected string GetVesselBiome()
        {
            switch (vessel.situation)
            {
                case Vessel.Situations.LANDED:
                case Vessel.Situations.PRELAUNCH:
                    return vessel.landedAt;
                case Vessel.Situations.ORBITING:
                    return "Orbit";
                default:
                    return string.Empty;
            }
        }

        public override void StartResourceConverter()
        {
            if (!IsConnectedToDepot)
                WAPM.DisplayMessage(NOT_CONNECTED_MESSAGE);
            else
                base.StartResourceConverter();
        }

        public override void OnAwake()
        {
            base.OnAwake();

            IsStandaloneConverter = true;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            var scenario = HighLogic.FindObjectOfType<WOLF_ScenarioModule>();
            _depotRegistry = scenario.ServiceManager.GetService<IDepotRegistry>();

            ParseWolfRecipe();

            // If we were previously connected to a depot, make sure we still are
            if (IsConnectedToDepot)
            {
                var body = vessel.mainBody.name;
                var biome = GetVesselBiome();
                var depot = _depotRegistry.GetDepot(body, biome);

                if (depot == null)
                {
                    WAPM.DisplayMessage(LOST_CONNECTION_MESSAGE);
                    IsConnectedToDepot = false;
                    StopResourceConverter();
                }
                else if (depot.Body != body || depot.Biome != biome)
                {
                    WAPM.DisplayMessage(LOST_CONNECTION_MESSAGE);
                    IsConnectedToDepot = false;
                    StopResourceConverter();

                    var resourcesToRelease = new Dictionary<string, int>();
                    foreach (var input in _wolfRecipe.InputIngredients)
                    {
                        resourcesToRelease.Add(input.Key, input.Value * -1);
                    }

                    var result = depot.NegotiateConsumer(resourcesToRelease);
                    if (result is FailedNegotiationResult)
                    {
                        Debug.LogError("[WOLF] Could not release hopper resources back to depot.");
                    }
                }
            }
        }

        protected void ParseWolfRecipe()
        {
            var inputIngredients = WAPM.ParseRecipeIngredientList(InputResources);
            if (inputIngredients == null)
            {
                return;
            }

            _wolfRecipe = new Recipe(inputIngredients, new Dictionary<string, int>());
        }
    }
}
