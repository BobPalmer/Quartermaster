using System.Linq;

namespace WOLF
{
    [KSPModule("Harvester")]
    public class WOLF_HarvesterModule : WOLF_ConverterModule
    {
        private int CalculateAbundance(string resourceName)
        {
            return 1;
        }

        public override string GetInfo()
        {
            if (part.FindModulesImplementing<WOLF_RecipeOption>().Any())
            {
                return string.Empty;
            }
            else
            {
                return base.GetInfo();
            }
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
