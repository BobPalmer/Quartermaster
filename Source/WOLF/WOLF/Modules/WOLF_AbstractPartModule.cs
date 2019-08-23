using KSP.Localization;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using Situations = Vessel.Situations;

namespace WOLF
{
    [KSPModule("Converter")]
    public abstract class WOLF_AbstractPartModule : PartModule
    {
        protected double _nextBiomeUpdate = 0d;
        protected IRegistryCollection _registry;

        protected static string CURRENT_BIOME_GUI_NAME = "#autoLOC_USI_WOLF_CURRENT_BIOME_GUI_NAME"; // "Current biome";
        protected static readonly List<string> KSC_BIOMES = new List<string>
        {
            "KSC",
            "Runway",
            "LaunchPad"
        };
        protected static string NEEDS_TEXT = "#autoLOC_USI_WOLF_NEEDS";  // "Needs"
        protected static string PROVIDES_TEXT = "#autoLOC_USI_WOLF_PROVIDES";  // "Provides"

        public IRecipe Recipe { get; private set; }

        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Current biome test", isPersistant = false)]
        public string CurrentBiome = "???";

        [KSPField]
        public string PartInfo = "Input something, get something else out.";

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

        public void ChangeRecipe(string inputResources, string outputResources)
        {
            ParseRecipe(inputResources, outputResources);
        }

        protected void DisplayMessage(string message)
        {
            Messenger.DisplayMessage(message);
        }

        public override string GetInfo()
        {
            var info = new StringBuilder();
            info
                .AppendLine(PartInfo)
                .AppendLine();

            if (Recipe == null)
                ParseRecipe();

            if (Recipe.InputIngredients.Count > 0)
            {
                info.AppendFormat("<color=#99FF00>{0}:</color>", NEEDS_TEXT);
                info.AppendLine();
                foreach (var resource in Recipe.InputIngredients)
                {
                    info
                        .Append(" - ")
                        .Append(resource.Key)
                        .Append(": ")
                        .AppendFormat("{0:D}", resource.Value)
                        .AppendLine();
                }
            }
            if (Recipe.OutputIngredients.Count > 0)
            {
                info.AppendFormat("<color=#99FF00>{0}:</color>", PROVIDES_TEXT);
                info.AppendLine();
                foreach (var resource in Recipe.OutputIngredients)
                {
                    info
                        .Append(" - ")
                        .Append(resource.Key)
                        .Append(": ")
                        .AppendFormat("{0:D}", resource.Value)
                        .AppendLine();
                }
            }

            return info.ToString();
        }

        protected string GetVesselBiome()
        {
            vessel.checkLanded();
            switch (vessel.situation)
            {                
                case Situations.LANDED:
                case Situations.PRELAUNCH:
                    if (string.IsNullOrEmpty(vessel.landedAt))
                        return ScienceUtil.GetExperimentBiome(vessel.mainBody, vessel.latitude, vessel.longitude);
                    else
                        return GetVesselLandedAtBiome(vessel.landedAt);
                case Situations.ORBITING:
                    return "Orbit";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// If landed at or near KSC, this consolidates all KSC mini biomes down to a single biome.
        /// </summary>
        /// <param name="landedAt"><see cref="Vessel.landedAt"/></param>
        /// <returns></returns>
        protected string GetVesselLandedAtBiome(string landedAt)
        {
            foreach (var biome in KSC_BIOMES)
            {
                if (landedAt.StartsWith(biome))
                    return "KSC";
            }
            return landedAt;
        }

        public override void OnAwake()
        {
            base.OnAwake();

            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_NEEDS", out string needsText))
            {
                NEEDS_TEXT = needsText;
            }
            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_PROVIDES", out string providesText))
            {
                PROVIDES_TEXT = providesText;
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_CURRENT_BIOME_GUI_NAME", out string currentBiomeGuiName))
            {
                CURRENT_BIOME_GUI_NAME = currentBiomeGuiName;
            }
            Fields["CurrentBiome"].guiName = CURRENT_BIOME_GUI_NAME;

            var scenario = FindObjectOfType<WOLF_ScenarioModule>();
            _registry = scenario.ServiceManager.GetService<IRegistryCollection>();

            ParseRecipe();
        }

        protected void ParseRecipe()
        {
            ParseRecipe(InputResources, OutputResources);
        }

        protected void ParseRecipe(string inputResources, string outputResources)
        {
            var inputIngredients = ParseRecipeIngredientList(inputResources);
            var outputIngredients = ParseRecipeIngredientList(outputResources);

            // If inputs or outputs were null, that means there was an error parsing the ingredients
            if (inputIngredients == null || outputIngredients == null)
            {
                return;
            }

            Recipe = new Recipe(inputIngredients, outputIngredients);
        }

        public static Dictionary<string, int> ParseRecipeIngredientList(string ingredients)
        {
            var ingredientList = new Dictionary<string, int>();
            if (!string.IsNullOrEmpty(ingredients))
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

        void Update()
        {
            // Display current biome in PAW
            if (HighLogic.LoadedSceneIsFlight)
            {
                var now = Planetarium.GetUniversalTime();
                if (now >= _nextBiomeUpdate)
                {
                    _nextBiomeUpdate = now + 1d;  // wait one second between biome updates
                    CurrentBiome = GetVesselBiome();
                }
            }
        }
    }
}
