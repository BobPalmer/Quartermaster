using System.Collections.Generic;

namespace Quartermaster
{
    public interface IGameData
    {
        double GetUniversalTime();
        bool LoadedSceneIsFlight();
        bool LoadedSceneIsEditor();
        List<string> GetAllVesselIds();
        string GetVesselName(string id);
        string GetPlanetName(int idx);
    }
}