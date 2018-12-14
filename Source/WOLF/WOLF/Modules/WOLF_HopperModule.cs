using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using USITools;

namespace WOLF
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
            // Check for issues that would prevent deployment
            if (IsConnectedToDepot)
            {
                Messenger.DisplayMessage(ALREADY_CONNECTED_MESSAGE);
                return;
            }

            var body = vessel.mainBody.name;
            var biome = GetVesselBiome();

            if (biome == string.Empty)
            {
                Messenger.DisplayMessage(Messenger.INVALID_SITUATION_MESSAGE);
                return;
            }
            if (!_depotRegistry.HasDepot(body, biome))
            {
                Messenger.DisplayMessage(Messenger.MISSING_DEPOT_MESSAGE);
                return;
            }
            var otherDepotModules = vessel.FindPartModulesImplementing<WOLF_DepotModule>();
            if (otherDepotModules.Any())
            {
                Messenger.DisplayMessage(Messenger.INVALID_DEPOT_PART_ATTACHMENT_MESSAGE);
                return;
            }
            var otherWolfPartModules = vessel.FindPartModulesImplementing<WOLF_AbstractPartModule>();
            if (otherWolfPartModules.Any())
            {
                Messenger.DisplayMessage(Messenger.INVALID_HOPPER_PART_ATTACHMENT_MESSAGE);
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
                    Messenger.DisplayMessage(string.Format(Messenger.MISSING_RESOURCE_MESSAGE, missingResource.Value, missingResource.Key));
                }
                return;
            }

            DepotBody = body;
            DepotBiome = biome;
            IsConnectedToDepot = true;

            Messenger.DisplayMessage(string.Format(Messenger.SUCCESSFUL_DEPLOYMENT_MESSAGE, body));
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
                Messenger.DisplayMessage(NOT_CONNECTED_MESSAGE);
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

            var scenario = FindObjectOfType<WOLF_ScenarioModule>();
            _depotRegistry = scenario.ServiceManager.GetService<IRegistryCollection>();

            ParseWolfRecipe();

            // If we were previously connected to a depot, make sure we still are
            if (IsConnectedToDepot)
            {
                var body = vessel.mainBody.name;
                var biome = GetVesselBiome();
                var depot = _depotRegistry.GetDepot(body, biome);

                if (depot == null)
                {
                    Messenger.DisplayMessage(LOST_CONNECTION_MESSAGE);
                    IsConnectedToDepot = false;
                    StopResourceConverter();
                }
                else if (depot.Body != body || depot.Biome != biome)
                {
                    Messenger.DisplayMessage(LOST_CONNECTION_MESSAGE);
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
            var inputIngredients = WOLF_AbstractPartModule.ParseRecipeIngredientList(InputResources);
            if (inputIngredients == null)
            {
                return;
            }

            _wolfRecipe = new Recipe(inputIngredients, new Dictionary<string, int>());
        }
    }
}
