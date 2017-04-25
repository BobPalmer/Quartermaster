namespace Quartermaster.Tests.Unit
{
    public class FakeNetwork : IResourceNetworkProvider
    {
        private NetworkRepository _repo;
        public NetworkRepository Repo => _repo ?? (_repo = new NetworkRepository(new FakePersister()));

        public void ResetCache()
        {
            Repo.ResetCache();
        }

        public void SaveLink(ResourceLink link)
        {
            Repo.SaveLink(link);
        }
    }
}