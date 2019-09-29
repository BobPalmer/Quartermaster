using KSP.Localization;

namespace WOLF
{
    public static class RewardsManager
    {
        private static readonly double FUNDS_PER_PAYLOAD_UNIT = 100d;
        private static readonly float SCIENCE_PER_BIOME = 100f;
        private static readonly float REP_PER_CREW_POINT = 10f;

        private static readonly string FUNDS_ADDED_MESSAGE = "#autoLOC_USI_WOLF_REWARDS_FUNDS_ADDED_MESSAGE";  // "You gained {0} Funds!";
        private static readonly string REPUTATION_ADDED_MESSAGE = "#autoLOC_USI_WOLF_REWARDS_SCIENCE_ADDED_MESSAGE";  // "You gained {0} Science!";
        private static readonly string SCIENCE_ADDED_MESSAGE = "#autoLOC_USI_WOLF_REWARDS_REPUTATION_ADDED_MESSAGE";  // "You gained {0} Reputation!";

        static RewardsManager()
        {
            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_REWARDS_FUNDS_ADDED_MESSAGE", out string fundsAddedMessage))
            {
                FUNDS_ADDED_MESSAGE = fundsAddedMessage;
            }
            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_REWARDS_REPUTATION_ADDED_MESSAGE", out string repAddedMessage))
            {
                REPUTATION_ADDED_MESSAGE = repAddedMessage;
            }
            if (Localizer.TryGetStringByTag("#autoLOC_USI_WOLF_REWARDS_SCIENCE_ADDED_MESSAGE", out string scienceAddedMessage))
            {
                SCIENCE_ADDED_MESSAGE = scienceAddedMessage;
            }
        }

        public static void AddFunds(int payload)
        {
            if (Funding.Instance != null)
            {
                var funds = FUNDS_PER_PAYLOAD_UNIT * payload;
                Funding.Instance.AddFunds(funds, TransactionReasons.ContractReward);
                Messenger.DisplayMessage(string.Format(FUNDS_ADDED_MESSAGE, funds.ToString("F0")));
            }
        }

        public static void AddReputation(int crewPoints)
        {
            if (Reputation.Instance != null)
            {
                var rep = REP_PER_CREW_POINT * crewPoints;
                Reputation.Instance.AddReputation(rep, TransactionReasons.ContractReward);
                Messenger.DisplayMessage(string.Format(REPUTATION_ADDED_MESSAGE, rep.ToString("F0")));
            }
        }

        public static void AddScience()
        {
            if (ResearchAndDevelopment.Instance != null)
            {
                ResearchAndDevelopment.Instance.AddScience(SCIENCE_PER_BIOME, TransactionReasons.ContractReward);
                Messenger.DisplayMessage(string.Format(SCIENCE_ADDED_MESSAGE, SCIENCE_PER_BIOME.ToString("F0")));
            }
        }
    }
}
