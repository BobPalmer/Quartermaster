using System.Collections.Generic;

namespace Quartermaster
{
    public class ResourcePool
    {
        private string _vesselId;
        private string _poolId;
        private bool _isLanded;
        private int _mainBody;
        private EndpointTypes _type;

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

        public int MainBodyIndex
        {
            get { return _mainBody; }
            set { _mainBody = value; }
        }

        public bool IsLanded
        {
            get { return _isLanded; }
            set { _isLanded = value; }
        }

        public EndpointTypes Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public ResourcePool(IResourceNetworkProvider net, string vId, string pId, bool landed, int idx, EndpointTypes type)
        {
            _network = net;
            _poolId = pId;
            _vesselId = vId;
            _mainBody = idx;
            _isLanded = landed;
            _type = type;

            var ep = new Endpoint
            {
                EndpointId = _poolId,
                MainBodyIndex = _mainBody,
                IsLanded = _isLanded,
                VesselId = _vesselId,
                Type = _type
            };

            _network.Instance.Repo.SaveEndpoint(ep);
        }

        public ResourcePool(string vId, string pId, bool landed, int idx, EndpointTypes type) : this(ResourceNetwork.Instance,vId, pId,landed,idx,type)
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