using System.Collections.Generic;

namespace WOLF
{
    public interface IRecipe
    {
        Dictionary<string, RecipeIngredient> InputIngredients { get; }
        Dictionary<string, RecipeIngredient> OutputIngredients { get; }
    }
}
