using Quartermaster.Persistence;

namespace Quartermaster.Tests.Unit
{
    public class FakeNetwork : IResourceNetworkProvider
    {
        private NetworkRepository _repo;
        private static FakeNetwork _instance;

        public NetworkRepository Repo => _repo ?? 
            (_repo = new NetworkRepository(new FakePersister(),new FakeGameInterface()));

        public static FakeNetwork Instance => _instance ??
                                                  (_instance = new FakeNetwork());
        IResourceNetworkProvider IResourceNetworkProvider.Instance => Instance;
    }
}