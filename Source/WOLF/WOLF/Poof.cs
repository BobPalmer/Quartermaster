using System.Collections.Generic;

namespace WOLF
{
    public static class Poof
    {
        public static bool CanGoPoof(Vessel vessel)
        {
            var crew = vessel.GetVesselCrew();

            return (crew == null || crew.Count < 1);
        }

        public static void GoPoof(Vessel vessel)
        {
            if (CanGoPoof(vessel))
            {
                foreach (var part in vessel.parts.ToArray())
                {
                    part.Die();
                }
                vessel.Die();
            }
        }
    }
}
