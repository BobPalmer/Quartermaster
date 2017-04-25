namespace Quartermaster
{
    public interface IGameData
    {
        double GetUniversalTime();
        bool LoadedSceneIsFlight();
        bool LoadedSceneIsEditor();
    }
}