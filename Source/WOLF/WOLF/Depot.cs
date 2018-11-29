using System.Collections.Generic;

namespace WOLF
{
    public class Depot : IDepot, IContractNegotiator
    {
        protected Dictionary<string, IResourceStream> _resourceStreams = new Dictionary<string, IResourceStream>();
        protected static readonly string _depotNodeName = "DEPOT";
        protected static readonly string _streamNodeName = "RESOURCE";

        public string Body { get; private set; }
        public string Biome { get; private set; }
        public Vessel.Situations Situation { get; private set; }

        public Depot(string body, string biome, Vessel.Situations situation)
        {
            Body = body;
            Biome = biome;
            Situation = Situation;
        }

        public NegotiationResult Negotiate(IRecipe recipe)
        {
            return Negotiate(recipe.InputIngredients, recipe.OutputIngredients);
        }

        public NegotiationResult Negotiate(Dictionary<string, int> consumedResources, Dictionary<string, int> providedResources)
        {
            var consumerResult = NegotiateConsumer(consumedResources);

            if (consumerResult is FailedNegotiationResult)
                return consumerResult;

            return NegotiateProvider(providedResources);
        }

        public NegotiationResult NegotiateProvider(Dictionary<string, int> providedResources)
        {
            foreach (var resource in providedResources)
            {
                IResourceStream stream;
                if (_resourceStreams.ContainsKey(resource.Key))
                {
                    stream = _resourceStreams[resource.Key];
                }
                else
                {
                    stream = new ResourceStream(resource.Key);
                    _resourceStreams.Add(resource.Key, stream);
                }

                stream.Incoming += resource.Value;
            }

            return new OkNegotiationResult();
        }

        public NegotiationResult NegotiateConsumer(Dictionary<string, int> consumedResources)
        {
            Dictionary<string, int> missingResources = new Dictionary<string, int>();
            foreach (var resource in consumedResources)
            {
                // Make sure we actually have the requested resource
                if (!_resourceStreams.ContainsKey(resource.Key))
                {
                    missingResources.Add(resource.Key, resource.Value);
                }
                // Make sure we have enough of the requested resource
                else if (_resourceStreams[resource.Key].Available < resource.Value)
                {
                    var stream = _resourceStreams[resource.Key];
                    missingResources.Add(resource.Key, resource.Value - stream.Available);
                }
            }

            if (missingResources.Count > 0)
            {
                return new FailedNegotiationResult(missingResources);
            }

            // We have the necessary resources, so proceed with negotiation
            foreach (var resource in consumedResources)
            {
                var stream = _resourceStreams[resource.Key];
                stream.Outgoing += resource.Value;
            }

            return new OkNegotiationResult();
        }

        public void OnLoad(ConfigNode node)
        {
            var streamNodes = node.GetNodes();
            foreach (var streamNode in streamNodes)
            {
                var resourceName = streamNode.GetValue("ResourceName");
                if (!_resourceStreams.ContainsKey(resourceName))
                {
                    var stream = new ResourceStream(resourceName);
                    stream.Incoming = int.Parse(streamNode.GetValue("Incoming"));
                    stream.Outgoing = int.Parse(streamNode.GetValue("Outgoing"));

                    _resourceStreams.Add(resourceName, stream);
                }
            }
        }

        public void OnSave(ConfigNode node)
        {
            if (_resourceStreams.Count > 0)
            {
                var depotNode = node.AddNode(_depotNodeName);
                foreach (var stream in _resourceStreams)
                {
                    var streamNode = depotNode.AddNode(_streamNodeName);
                    var streamValue = stream.Value;
                    streamNode.AddValue("ResourceName", streamValue.ResourceName);
                    streamNode.AddValue("Incoming", streamValue.Incoming);
                    streamNode.AddValue("Outgoing", streamValue.Outgoing);
                }
            }
        }
    }
}
