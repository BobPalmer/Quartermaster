namespace WOLF
{
    public class WOLF_RecipeOption : PartModule
    {
        [KSPField]
        public string RecipeDisplayName;

        [KSPField]
        public string InputResources = string.Empty;

        [KSPField]
        public string OutputResources = string.Empty;
    }
}
