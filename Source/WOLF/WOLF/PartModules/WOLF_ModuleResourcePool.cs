using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Quartermaster.PartModules
{
    public class WOLF_ModuleResourcePool : PartModule
    {
        [KSPField(isPersistant = true)]
        public string PoolId;

        private ResourcePool _pool;

        public ResourcePool Pool
        {
            get { return _pool ?? (_pool = SetupPool()); }
        }

        private ResourcePool SetupPool()
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return null;
            if (String.IsNullOrEmpty(PoolId))
                PoolId = Guid.NewGuid().ToString();

            vessel.UpdateLandedSplashed();
            return new ResourcePool(vessel.id.ToString(),PoolId,vessel.LandedOrSplashed,vessel.mainBody.flightGlobalsIndex,EndpointTypes.Pool);
        }
    }
}
