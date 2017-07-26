using System.Collections.Generic;

namespace Quartermaster
{
    public class ResourcePool
    {
        private string _vesselId;
        private string _poolId;
        private IResourceNetworkProvider _network;

        public string VesselId
        {
            get { return _vesselId; }
            set { _vesselId = value; }
        }

        public string PoolId
        {
            get { return _poolId; }
            set { _poolId = value; }
        }

        public ResourcePool(IResourceNetworkProvider net, string vId, string pId)
        {
            _network = net;
            _poolId = pId;
            _vesselId = vId;

            var ep = new Endpoint
            {
                EndpointId = _poolId,
                VesselId = _vesselId
            };

            _network.Instance.Repo.SaveEndpoint(ep);
        }

        public ResourcePool(string vId, string pId) : this(ResourceNetwork.Instance,vId, pId)
        { }

        public bool CheckResources(Recipe r)
        {
            if (MissingResources(r.Inputs))
                return false;
            if (MissingResources(r.Requirements))
                return false;

            return true;
        }

        private bool MissingResources(List<Ingredient> iList)
        {
            var count = iList.Count;
            for (int i = 0; i < count; ++i)
            {
                var ing = iList[i];
                var amount = _network.Instance.Repo.GetResourceQuantity(PoolId, ing.ResourceName);
                if(amount < ing.Quantity)
                    return true;
            }
            return false;
        } 
    }
}