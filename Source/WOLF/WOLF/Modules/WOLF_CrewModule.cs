using System.Collections.Generic;

namespace WOLF
{
    public class WOLF_CrewModule : VesselModule
    {
        private static readonly int REQUIRED_LIFE_SUPPORT = 1;
        private static readonly int REQUIRED_HABITATION = 1;
        private static readonly int REQUIRED_CAFETERIA = 1;
        private static readonly int MULCH_OUTPUT = 1;
        private static readonly string RESOURCE_NAME_LIFESUPPORT = "LifeSupport";
        private static readonly string RESOURCE_NAME_HABITATION = "Habitation";
        private static readonly string RESOURCE_NAME_CAFETERIA = "Cafeteria";
        private static readonly string RESOURCE_NAME_MULCH = "Mulch";

        public static readonly string CREW_RESOURCE_SUFFIX = "CrewPoint";

        public IRecipe GetCrewRecipe()
        {
            var roster = vessel.GetVesselCrew();
            if (roster.Count < 1)
                return new Recipe();

            var inputs = new Dictionary<string, int>
            {
                { RESOURCE_NAME_LIFESUPPORT, 0 },
                { RESOURCE_NAME_HABITATION, 0 },
                { RESOURCE_NAME_CAFETERIA, 0 }
            };
            var outputs = new Dictionary<string, int>();
            foreach (var kerbal in roster)
            {
                inputs[RESOURCE_NAME_LIFESUPPORT] += REQUIRED_LIFE_SUPPORT;
                inputs[RESOURCE_NAME_HABITATION] += REQUIRED_HABITATION;
                inputs[RESOURCE_NAME_CAFETERIA] += REQUIRED_CAFETERIA;

                var resourceName = kerbal.trait + CREW_RESOURCE_SUFFIX;
                var stars = kerbal.experienceLevel;
                if (!outputs.ContainsKey(resourceName))
                {
                    outputs.Add(resourceName, stars);
                }
                else
                {
                    outputs[resourceName] += stars;
                }
                if (!outputs.ContainsKey(RESOURCE_NAME_MULCH))
                {
                    outputs.Add(RESOURCE_NAME_MULCH, MULCH_OUTPUT);
                }
                else
                {
                    outputs[RESOURCE_NAME_MULCH] += MULCH_OUTPUT;
                }
            }

            return new Recipe(inputs, outputs);
        }
    }
}
