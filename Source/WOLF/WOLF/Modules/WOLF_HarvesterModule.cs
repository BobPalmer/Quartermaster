namespace WOLF
{
    public class WOLF_HarvesterModule : WOLF_ConverterModule
    {
        private int CalculateAbundance(string resourceName)
        {
            return 1;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (Recipe.OutputIngredients.Count > 0)
            {
                var resources = new string[Recipe.OutputIngredients.Count];
                Recipe.OutputIngredients.Keys.CopyTo(resources, 0);
                foreach (var resource in resources)
                {
                    var abundance = CalculateAbundance(resource);

                    Recipe.OutputIngredients[resource] *= abundance;
                }
            }
        }
    }
}
