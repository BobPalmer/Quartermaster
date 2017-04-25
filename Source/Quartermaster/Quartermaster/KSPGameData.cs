namespace Quartermaster
{
    public class KSPGameData : IGameData
    {
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