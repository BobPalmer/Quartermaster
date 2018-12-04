using System.Collections.Generic;
using UnityEngine;

namespace WOLF
{
    public class WOLF_DepotModule : PartModule
    {
        private static readonly string DEPOT_ALREADY_ESTABLISHED_MESSAGE = "A depot has already been established here!";
        private static readonly string INVALID_SITUATION_MESSAGE = "Can only estabish a depot when landed on the surface or in orbit.";
        private static readonly string MISSING_CREW_MESSAGE = "Need {0} more Kerbal(s) with {1} trait.";
        private static readonly string RECIPE_PARSE_FAILURE_MESSAGE = "[WOLF] Error parsing DepotModule recipe ingredients. Check part config.";
        private static readonly string SUCCESSFUL_DEPLOYMENT_MESSAGE = "Your depot has been established at {0} on {1}!";
        private static readonly float SCREEN_MESSAGE_DURATION = 5f;

        private IDepotRegistry _depotRegistry;
        private IRecipe _recipe;

        [KSPField]
        public string InputResources = "";

        [KSPField]
        public string OutputResources = "";

        [KSPEvent(guiName = "Establish Depot", active = true, guiActive = true, guiActiveEditor = false)]
        public void EstablishDepot()
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
            depot.NegotiateProvider(_recipe.OutputIngredients);
            depot.NegotiateConsumer(_recipe.InputIngredients);

            // TODO - Backup crew roster
            // TODO - Make vessel go poof (or replace with a single-part vessel that can be expanded with various hoppers for resource/crew retrieval?)

            DisplayMessage(string.Format(SUCCESSFUL_DEPLOYMENT_MESSAGE, biome, body));
        }

        private Dictionary<string, int> CheckForMissingCrew()
        {
            return new Dictionary<string, int>();
        }

        private string GetVesselBiome()
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

        private void DisplayMessage(string message)
        {
            ScreenMessages.PostScreenMessage(message, SCREEN_MESSAGE_DURATION, ScreenMessageStyle.UPPER_CENTER);
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            var scenario = HighLogic.FindObjectOfType<WOLF_ScenarioModule>();
            _depotRegistry = scenario.ServiceManager.GetService<IDepotRegistry>();

            ParseRecipe();
        }

        private void ParseRecipe()
        {
            var inputIngredients = ParseRecipeIngredientList(InputResources);
            var outputIngredients = ParseRecipeIngredientList(OutputResources);

            if (inputIngredients == null || outputIngredients == null)
            {
                return;
            }

            _recipe = new Recipe(inputIngredients, outputIngredients);
        }

        private Dictionary<string, int> ParseRecipeIngredientList(string ingredients)
        {
            var ingredientList = new Dictionary<string, int>();
            if (ingredients != null && ingredients != string.Empty)
            {
                var tokens = ingredients.Split(',');
                if (tokens.Length % 2 != 0)
                {
                    Debug.LogError(RECIPE_PARSE_FAILURE_MESSAGE);
                    return null;
                }
                for (int i = 0; i < tokens.Length - 1; i = i + 2)
                {
                    var resource = tokens[i];
                    var quantityString = tokens[i + 1];

                    if (!PartResourceLibrary.Instance.resourceDefinitions.Contains(resource))
                    {
                        Debug.LogError(RECIPE_PARSE_FAILURE_MESSAGE);
                        return null;
                    }
                    if (!int.TryParse(quantityString, out int quantity))
                    {
                        Debug.LogError(RECIPE_PARSE_FAILURE_MESSAGE);
                        return null;
                    }

                    ingredientList.Add(resource, quantity);
                }
            }

            return ingredientList;
        }
    }
}
