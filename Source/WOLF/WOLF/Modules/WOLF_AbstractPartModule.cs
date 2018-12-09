using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WOLF
{
    public abstract class WOLF_AbstractPartModule : PartModule
    {
        protected IDepotRegistry _depotRegistry;

        public IRecipe Recipe { get; protected set; }

        [KSPField]
        public string InputResources = string.Empty;

        [KSPField]
        public string OutputResources = string.Empty;

        [KSPEvent(guiName = "Connect to WOLF", active = true, guiActive = true, guiActiveEditor = false)]
        public void ConnectToDepotEvent()
        {
            ConnectToDepot();
        }

        protected abstract void ConnectToDepot();

        protected void DisplayMessage(string message)
        {
            Messenger.DisplayMessage(message);
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

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            var scenario = FindObjectOfType<WOLF_ScenarioModule>();
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
                    Debug.LogError(Messenger.RECIPE_PARSE_FAILURE_MESSAGE);
                    return null;
                }
                for (int i = 0; i < tokens.Length - 1; i = i + 2)
                {
                    var resource = tokens[i];
                    var quantityString = tokens[i + 1];

                    if (!PartResourceLibrary.Instance.resourceDefinitions.Contains(resource)
                        && !WOLF_ScenarioModule.AuxillaryResources.Contains(resource))
                    {
                        Debug.LogError(Messenger.RECIPE_PARSE_FAILURE_MESSAGE);
                        return null;
                    }
                    if (!int.TryParse(quantityString, out int quantity))
                    {
                        Debug.LogError(Messenger.RECIPE_PARSE_FAILURE_MESSAGE);
                        return null;
                    }

                    ingredientList.Add(resource, quantity);
                }
            }

            return ingredientList;
        }
    }
}
