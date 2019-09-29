namespace WOLF
{
    public class WOLF_GameParameters : GameParameters.CustomParameterNode
    {
        [GameParameters.CustomIntParameterUI(
            "#autoLOC_WOLF_RESOURCE_ABUNDANCE_MULTIPLIER_OPTION_TITLE",
            autoPersistance = true, minValue = 1, maxValue = 10, stepSize = 1,
            toolTip = "#autoLOC_WOLF_RESOURCE_ABUNDANCE_MULTIPLIER_OPTION_TOOLTIP")]
        public int ResourceAbundanceMultiplier = 1;

        public static int ResourceAbundanceMultiplierValue
        {
            get
            {
                var options = HighLogic.CurrentGame.Parameters.CustomParams<WOLF_GameParameters>();

                return options.ResourceAbundanceMultiplier;
            }
        }

        public override string Section => "WOLF";

        public override string DisplaySection => "WOLF";

        public override string Title => string.Empty;

        public override int SectionOrder => 1;

        public override GameParameters.GameMode GameMode => GameParameters.GameMode.ANY;

        public override bool HasPresets => false;
    }
}
