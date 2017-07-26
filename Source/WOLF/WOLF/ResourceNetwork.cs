using System;
using Quartermaster.Persistence;
using UnityEngine;
using USITools;


namespace Quartermaster
{
    public class ResourceNetwork : MonoBehaviour, IResourceNetworkProvider
    {
        private static ResourceNetwork _instance;
        private NetworkRepository _repo;

        public static ResourceNetwork Instance => _instance ?? 
                                                  (_instance = new GameObject("ResourceNetwork").AddComponent<ResourceNetwork>());

        public NetworkRepository Repo => _repo ??
//TODO:  Fix this for PROD!              (_repo = new NetworkRepository(new NetworkPersistence()));
                                         (_repo = new NetworkRepository(new FakePersister()));

        IResourceNetworkProvider IResourceNetworkProvider.Instance => Instance;
    }
}

