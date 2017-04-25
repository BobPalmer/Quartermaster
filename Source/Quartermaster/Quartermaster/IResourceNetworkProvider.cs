using JetBrains.Annotations;

namespace Quartermaster
{
    public interface IResourceNetworkProvider
    {
        NetworkRepository Repo { get; }
    }
}