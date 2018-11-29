﻿using System.Collections.Generic;

namespace WOLF
{
    public class Recipe : IRecipe
    {
        public Dictionary<string, int> InputIngredients { get; private set; } = new Dictionary<string, int>();
        public Dictionary<string, int> OutputIngredients { get; private set; } = new Dictionary<string, int>();
    }
}
