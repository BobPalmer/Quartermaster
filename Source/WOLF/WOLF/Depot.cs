﻿using System.Collections.Generic;
using System.Linq;

namespace WOLF
{
    public class Depot : IDepot
    {
        protected Dictionary<string, IResourceStream> _resourceStreams = new Dictionary<string, IResourceStream>();
        protected static readonly string _depotNodeName = "DEPOT";
        protected static readonly string _streamNodeName = "RESOURCE";

        public string Body { get; private set; }
        public string Biome { get; private set; }

        public Depot(string body, string biome)
        {
            Body = body;
            Biome = biome;
        }

        private Dictionary<string, int> CheckForMissingResources(string resourceName, int quantity)
        {
            var missingResources = new Dictionary<string, int>();

            return missingResources;
        }

        public List<IResourceStream> GetResources()
        {
            return _resourceStreams.Select(s => s.Value).ToList();
        }

        public NegotiationResult Negotiate(IRecipe recipe)
        {
            return Negotiate(recipe.InputIngredients, recipe.OutputIngredients);
        }

        /// <summary>
        /// Use this method to negotiate multiple recipes at the same time.
        /// </summary>
        /// <param name="recipes"></param>
        /// <returns></returns>
        public NegotiationResult Negotiate(List<IRecipe> recipes)
        {
            // Our goal is to reduce everything down to a single recipe
            // Start by combining all the ins and outs
            var inputIngredients = recipes
                .SelectMany(r => r.InputIngredients)
                .GroupBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Value));
            var outputIngredients = recipes
                .SelectMany(r => r.OutputIngredients)
                .GroupBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Value));

            // Reduce all ingredients down so that resources only appear once
            //   as either an input or an output
            var inputArray = inputIngredients.ToArray();
            for (int i = 0; i < inputArray.Length; i++)
            {
                var input = inputArray[i];
                var resourceName = input.Key;
                var inputQuantity = input.Value;
                if (outputIngredients.ContainsKey(resourceName))
                {
                    var outputQuantity = outputIngredients[input.Key];
                    if (inputQuantity > outputQuantity)
                    {
                        inputIngredients[resourceName] -= outputQuantity;
                        outputIngredients.Remove(resourceName);
                    }
                    else
                    {
                        outputIngredients[resourceName] -= inputQuantity;
                        inputIngredients.Remove(resourceName);
                    }
                }
            }

            // Now just proceed as if we're negotiating a single recipe!
            var recipe = new Recipe(inputIngredients, outputIngredients);
            return Negotiate(recipe);
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
            var missingResources = new Dictionary<string, int>();
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

            if (missingResources.Count() > 0)
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
            var depotNode = node.AddNode(_depotNodeName);
            depotNode.AddValue("Body", Body);
            depotNode.AddValue("Biome", Biome);

            if (_resourceStreams.Count > 0)
            {
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
