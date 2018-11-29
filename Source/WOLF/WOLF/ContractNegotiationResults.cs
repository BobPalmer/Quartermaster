using System.Collections.Generic;

namespace WOLF
{
    public abstract class NegotiationResult { }

    public class FailedNegotiationResult : NegotiationResult
    {
        public Dictionary<string, int> MissingResources { get; private set; }

        public FailedNegotiationResult(Dictionary<string, int> missingResources)
        {
            MissingResources = missingResources;
        }
    }

    public class OkNegotiationResult : NegotiationResult
    {
    }
}
