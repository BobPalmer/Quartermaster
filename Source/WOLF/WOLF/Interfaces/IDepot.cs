using System.Collections.Generic;

namespace WOLF
{
    public interface IDepot : IPersistenceAware
    {
        string Body { get; }
        string Biome { get; }
        Vessel.Situations Situation { get; }

        NegotiationResult NegotiateProvider(Dictionary<string, int> providedResources);
        NegotiationResult NegotiateConsumer(Dictionary<string, int> consumedResources);
    }
}
