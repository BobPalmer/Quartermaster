namespace Quartermaster.Tests.Unit
{
    public class FakeGameInterface : IGameData
    {
        private double _time;
        private bool _isFlight;
        private bool _isEditor;        

        public void SetUniversalTime(double time)
        {
            _time = time;
        }

        public void SetFlightState(bool state)
        {
            _isFlight = state;
            _isEditor = !state;
        }

        public void SetEditorState(bool state)
        {
            _isFlight = !state;
            _isEditor = state;
        }


        public double GetUniversalTime()
        {
            return _time;
        }

        public bool LoadedSceneIsFlight()
        {
            return _isFlight;
        }

        public bool LoadedSceneIsEditor()
        {
            return _isEditor;
        }
    }
}