using System.Collections.Generic;
using UnityEngine;

namespace WOLF
{
    public abstract class WOLF_AbstractPartModule : PartModule
    {
        public static readonly string RECIPE_PARSE_FAILURE_MESSAGE = "[WOLF] Error parsing DepotModule recipe ingredients. Check part config.";
        public static readonly float SCREEN_MESSAGE_DURATION = 5f;

        protected IDepotRegistry _depotRegistry;

        public IRecipe Recipe { get; protected set; }

        [KSPField]
        public string InputResources = "";

        [KSPField]
        public string OutputResources = "";

        [KSPEvent(guiName = "Connect to WOLF", active = true, guiActive = true, guiActiveEditor = false)]
        public void ConnectToDepotEvent()
        {
            ConnectToDepot();
        }

        protected abstract void ConnectToDepot();

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

        public static void DisplayMessage(string message)
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

        protected void ParseRecipe()
        {
            var inputIngredients = ParseRecipeIngredientList(InputResources);
            var outputIngredients = ParseRecipeIngredientList(OutputResources);

            if (inputIngredients == null || outputIngredients == null)
            {
                return;
            }

            Recipe = new Recipe(inputIngredients, outputIngredients);
        }

        public static Dictionary<string, int> ParseRecipeIngredientList(string ingredients)
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
