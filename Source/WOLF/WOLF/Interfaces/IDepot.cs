using System.Collections.Generic;

namespace WOLF
{
    public interface IDepot : IPersistenceAware
    {
        string Body { get; }
        string Biome { get; }

        List<IResourceStream> GetResources();
        NegotiationResult Negotiate(IRecipe recipe);
        NegotiationResult Negotiate(List<IRecipe> recipes);
        NegotiationResult NegotiateProvider(Dictionary<string, int> providedResources);
        NegotiationResult NegotiateConsumer(Dictionary<string, int> consumedResources);
    }
}
