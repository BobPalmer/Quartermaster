using System;
using UnityEngine;
using USITools;


namespace Quartermaster
{
    public class ResourceNetwork : MonoBehaviour, IResourceNetworkProvider
    {
        // Static singleton instance
        private static ResourceNetwork instance;

        // Static singleton property
        public static ResourceNetwork Instance
        {
            get { return instance ?? (instance = new GameObject("ResourceNetwork").AddComponent<ResourceNetwork>()); }
        }

        private NetworkRepository _repo;
        public NetworkRepository Repo
        {
            get { return _repo ?? (_repo = new 
                    NetworkRepository(new ResourcePersistence())); }
        }
    }
}

