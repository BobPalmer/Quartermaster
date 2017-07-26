using System.Collections.Generic;

namespace Quartermaster
{
    public class KSPGameData : IGameData
    {
        public List<string> GetAllVesselIds()
        {
            var vList = new List<string>();
            var count = FlightGlobals.Vessels.Count;
            for (int i = 0; i < count; ++i)
            {
                vList.Add(FlightGlobals.Vessels[i].id.ToString());
            }
            return vList;
        }

        public double GetUniversalTime()
        {
            return Planetarium.GetUniversalTime();
        }

        public bool LoadedSceneIsFlight()
        {
            return HighLogic.LoadedSceneIsFlight;
        }

        public bool LoadedSceneIsEditor()
        {
            return HighLogic.LoadedSceneIsEditor;
        }
    }
}