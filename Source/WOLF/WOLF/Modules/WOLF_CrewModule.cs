using System.Collections.Generic;
using System.Linq;

namespace WOLF
{
    public class WOLF_CrewModule : VesselModule
    {
        private static readonly int REQUIRED_LIFE_SUPPORT = 1;
        private static readonly int REQUIRED_HABITATION = 1;
        private static readonly int CO2_OUTPUT = 1;
        private static readonly int MULCH_OUTPUT = 1;
        private static readonly int WASTEWATER_OUTPUT = 1;
        private static readonly string RESOURCE_NAME_LIFESUPPORT = "LifeSupport";
        private static readonly string RESOURCE_NAME_HABITATION = "Habitation";
        private static readonly string RESOURCE_NAME_CO2 = "CarbonDioxide";
        private static readonly string RESOURCE_NAME_MULCH = "Mulch";
        private static readonly string RESOURCE_NAME_WASTEWATER = "WasteWater";

        public static readonly string CREW_RESOURCE_SUFFIX = "CrewPoint";

        public IRecipe GetCrewRecipe()
        {
            var roster = vessel.GetVesselCrew();
            if (roster.Count < 1)
                return new Recipe();

            var inputs = new Dictionary<string, int>
            {
                { RESOURCE_NAME_LIFESUPPORT, 0 },
                { RESOURCE_NAME_HABITATION, 0 }
            };
            var outputs = new Dictionary<string, int>
            {
                { RESOURCE_NAME_CO2, 0 },
                { RESOURCE_NAME_MULCH, 0 },
                { RESOURCE_NAME_WASTEWATER, 0 }
            };
            foreach (var kerbal in roster)
            {
                inputs[RESOURCE_NAME_LIFESUPPORT] += REQUIRED_LIFE_SUPPORT;
                inputs[RESOURCE_NAME_HABITATION] += REQUIRED_HABITATION;

                outputs[RESOURCE_NAME_CO2] += CO2_OUTPUT;
                outputs[RESOURCE_NAME_MULCH] += MULCH_OUTPUT;
                outputs[RESOURCE_NAME_WASTEWATER] += WASTEWATER_OUTPUT;

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
            }

            return new Recipe(inputs, outputs);
        }

        /// <summary>
        /// Only crew with at least 1 experience point are eligible to work at a WOLF colony!
        /// </summary>
        /// <returns></returns>
        public bool IsCrewEligible()
        {
            var roster = vessel.GetVesselCrew();
            if (roster.Count < 1)
                return true;

            return !roster.Any(c => c.experienceLevel < 1);
        }
    }
}
