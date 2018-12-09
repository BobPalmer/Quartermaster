using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WOLF
{
    public class WOLF_RecipeOptionController : PartModule
    {
        private static readonly string SWAP_SUCCESS_MESSAGE = "Reconfiguration from {0} to {1} completed.";

        private readonly List<WOLF_RecipeOption> _recipeOptions = new List<WOLF_RecipeOption>();
        private WOLF_AbstractPartModule _converter;
        private int _nextRecipeIndex;
        private bool _hasStartFinished = false;

        [KSPField(isPersistant = true)]
        private int selectedRecipeIndex;

        [KSPField(guiName = "Recipe", guiActive = true, guiActiveEditor = true)]
        private string selectedRecipeName = "???";

        [KSPEvent(guiName = "Switch to [None]", active = true, guiActive = true, guiActiveEditor = true, guiActiveUnfocused = true, unfocusedRange = 10f)]
        public void SwapRecipe()
        {
            var previousRecipeName = selectedRecipeName;
            selectedRecipeIndex = _nextRecipeIndex;
            MoveNext();

            Messenger.DisplayMessage(string.Format(SWAP_SUCCESS_MESSAGE, previousRecipeName, selectedRecipeName));

            ApplyRecipe();
        }

        [KSPEvent(guiName = "Next Recipe", active = true, guiActive = true, guiActiveEditor = true, guiActiveUnfocused = true, unfocusedRange = 10f)]
        public void MoveNext()
        {
            if (_recipeOptions.Count < 2)
            {
                return;
            }

            if (_hasStartFinished)
            {
                _nextRecipeIndex++;
            }
            else
            {
                _hasStartFinished = true;
                _nextRecipeIndex = selectedRecipeIndex + 1;
            }

            if (_nextRecipeIndex >= _recipeOptions.Count)
            {
                _nextRecipeIndex = 0;
            }
            if (_nextRecipeIndex == selectedRecipeIndex)
            {
                MoveNext();
            }

            UpdateMenu();
        }

        [KSPEvent(guiName = "Previous Recipe", active = true, guiActive = true, guiActiveEditor = true, guiActiveUnfocused = true, unfocusedRange = 10f)]
        public void MovePrevious()
        {
            if (_recipeOptions.Count < 2)
            {
                return;
            }

            _nextRecipeIndex--;
            if (_nextRecipeIndex < 0)
            {
                _nextRecipeIndex = _recipeOptions.Count - 1;
            }
            if (_nextRecipeIndex == selectedRecipeIndex)
            {
                MovePrevious();
            }

            UpdateMenu();
        }

        private void ApplyRecipe()
        {
            if (_converter != null)
            {
                var recipe = _recipeOptions[selectedRecipeIndex];
                _converter.ChangeRecipe(recipe.InputResources, recipe.OutputResources);
            }
            UpdateMenu();
        }

        public override void OnStart(StartState state)
        {
            var recipeOptions = part.FindModulesImplementing<WOLF_RecipeOption>();
            if (!recipeOptions.Any())
            {
                Debug.LogError(string.Format("[WOLF] {0}: Needs at least one WOLF_RecipeOption. Check part config.", GetType().Name));
            }

            _converter = part.FindModuleImplementing<WOLF_AbstractPartModule>();
            if (_converter == null)
            {
                Debug.LogError(string.Format("[WOLF] {0}: Needs a module derived from WOLF_AbstractPartModule. Check part config.", GetType().Name));
            }

            foreach (var option in recipeOptions)
            {
                _recipeOptions.Add(option);
            }

            ApplyRecipe();
            MoveNext();
        }

        private void UpdateMenu()
        {
            selectedRecipeName = _recipeOptions[selectedRecipeIndex].RecipeDisplayName;
            var nextRecipeName = _recipeOptions[_nextRecipeIndex].RecipeDisplayName;
            Events["SwapRecipe"].guiName = selectedRecipeName + " => " + nextRecipeName;

            MonoUtilities.RefreshContextWindows(part);
        }
    }
}
