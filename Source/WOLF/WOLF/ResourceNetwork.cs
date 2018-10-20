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
//TODO:  Fix this for PROD!              (_repo = new NetworkRepository(new NetworkPersistence(),new KSPGameData()));
                                         (_repo = new NetworkRepository(new FakePersister(),new KSPGameData()));

        IResourceNetworkProvider IResourceNetworkProvider.Instance => Instance;
    }
}

