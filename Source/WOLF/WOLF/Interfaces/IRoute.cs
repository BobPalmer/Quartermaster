using System.Collections.Generic;

namespace WOLF
{
    public interface IRoute : IPersistenceAware
    {
        string OriginBody { get; }
        string OriginBiome { get; }
        string DestinationBody { get; }
        string DestinationBiome { get; }
        int Payload { get; }

        Dictionary<string, int> GetResources();
        void IncreasePayload(int amount);
        NegotiationResult AddResource(string resourceName, int quantity);
        NegotiationResult RemoveResource(string resourceName, int quantity);
    }
}
