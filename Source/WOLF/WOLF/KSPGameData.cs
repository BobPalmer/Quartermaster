using System;
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

        public string GetPlanetName(int idx)
        {
            return FlightGlobals.Bodies[idx].bodyName;
        }

        public string GetVesselName(string id)
        {
            var count = FlightGlobals.Vessels.Count;
            for (int i = 0; i < count; ++i)
            {
                var v = FlightGlobals.Vessels[i];
                if (v.id == new Guid(id))
                    return v.vesselName;
            }
            return "??Unknown??";
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