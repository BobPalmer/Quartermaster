namespace WOLF
{
    public static class Messenger
    {
        public static readonly string INVALID_DEPOT_PART_ATTACHMENT_MESSAGE = "Depots must be detached from other WOLF parts before deployment.";
        public static readonly string INVALID_HOPPER_PART_ATTACHMENT_MESSAGE = "Hoppers must be detached from other WOLF parts before deployment.";
        public static readonly string INVALID_SITUATION_MESSAGE = "Your vessel must be landed or orbiting in order to connect to a depot.";
        public static readonly string MISSING_DEPOT_MESSAGE = "You must establish a depot in this biome first!";
        public static readonly string MISSING_RESOURCE_MESSAGE = "Depot needs an additional ({0}) {1}.";
        public static readonly string SUCCESSFUL_DEPLOYMENT_MESSAGE = "Your infrastructure has expanded on {0}!";
        public static readonly string RECIPE_PARSE_FAILURE_MESSAGE = "[WOLF] Error parsing recipe ingredients. Check part config.";

        public static readonly float SCREEN_MESSAGE_DURATION = 5f;

        public static void DisplayMessage(string message)
        {
            ScreenMessages.PostScreenMessage(message, SCREEN_MESSAGE_DURATION, ScreenMessageStyle.UPPER_CENTER);
        }
    }
}
