using System.Collections.Generic;

namespace WOLF
{
    public class Recipe : IRecipe
    {
        public Dictionary<string, RecipeIngredient> InputIngredients { get; private set; } = new Dictionary<string, RecipeIngredient>();
        public Dictionary<string, RecipeIngredient> OutputIngredients { get; private set; } = new Dictionary<string, RecipeIngredient>();
    }
}
