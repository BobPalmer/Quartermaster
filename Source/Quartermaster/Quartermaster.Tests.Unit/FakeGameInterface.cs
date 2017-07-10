using System.Collections.Generic;

namespace Quartermaster.Tests.Unit
{
    public class FakeGameInterface : IGameData
    {
        private double _time;
        private bool _isFlight;
        private bool _isEditor;
        private List<string> _vessels;

        public void ClearVesselList()
        {
            _vessels = new List<string>();
        }

        public void AddVessel(string v)
        {
            if(_vessels == null)
                _vessels = new List<string>();
            _vessels.Add(v);
        }

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

        public List<string> GetAllVesselIds()
        {
            throw new System.NotImplementedException();
        }
    }
}