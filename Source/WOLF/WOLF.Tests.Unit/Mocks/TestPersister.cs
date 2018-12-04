using System.Collections.Generic;

namespace WOLF.Tests.Unit.Mocks
{
    public class TestPersister : ScenarioPersister
    {
        /// <summary>
        /// Exposes the internal depot list for testing.
        /// </summary>
        public List<IDepot> Depots
        {
            get { return _depots; }
        }
    }
}
