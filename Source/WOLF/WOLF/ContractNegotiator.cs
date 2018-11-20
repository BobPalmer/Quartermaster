using System;
using System.Collections.Generic;
using System.Linq;

namespace WOLF
{
    public class ContractNegotiator : IContractNegotiator
    {
        private List<IContract> _contracts = new List<IContract>();
        private List<IEndpoint> _endpoints = new List<IEndpoint>();
        private ContractState[] _validOutgoingContractStates = new ContractState[] { ContractState.Active, ContractState.Broken };
        private static string _contractNodeName = "CONTRACT";
        private static string _endpointNodeName = "ENDPOINT";

        /// <summary>
        /// Registers an <see cref="IEndpoint"/> with the <see cref="ContractNegotiator"/>.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns>The assigned endpoint id.</returns>
        public string AddEndpoint(IEndpoint endpoint)
        {
            if (_endpoints.Contains(endpoint))
            {
                return endpoint.Id;
            }

            endpoint.Id = Guid.NewGuid().ToString();
            _endpoints.Add(endpoint);

            return endpoint.Id;
        }

        public IContract FindContract(string contractId)
        {
            return _contracts.Where(c => c.ContractId == contractId).FirstOrDefault();
        }

        public IEndpoint FindEndpoint(string endpointId)
        {
            return _endpoints.Where(e => e.Id == endpointId).FirstOrDefault();
        }

        public NegotiationResult Negotiate(string sourceId, string destinationId, string resourceName, double quantity, ContractRateUnit rate = ContractRateUnit.PerDay, double expiration = 0d)
        {
            var source = FindEndpoint(sourceId);
            var destination = FindEndpoint(destinationId);

            if (!source.CanProvide(resourceName, quantity, rate))
            {
                return new FailedNegotiationResult("Source cannot fulfill this request.");
            }

            var contract = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = source,
                Destination = destination,
                ResourceName = resourceName,
                Quantity = quantity,
                Rate = rate, 
                ExpirationTimestamp = expiration
            };

            _contracts.Add(contract);
            source.Contracts.Add(contract);
            destination.Contracts.Add(contract);

            // This will attempt to restore broken contracts
            ValidateContracts(destination.Id);

            return new OkNegotiationResult<Contract>(contract);
        }

        public void OnLoad(ConfigNode node)
        {
            var endpointNodes = node.GetNodes(_endpointNodeName);

            ConfigNode endpointNode;
            IEndpoint endpoint;
            for (int i = 0; i < endpointNodes.Length; i++)
            {
                endpointNode = endpointNodes[i];
                switch (endpointNode.GetValue("Type"))
                {
                    case "Processor":
                        endpoint = new ProcessorEndpoint { Id = }
                }
            }
        }

        public void OnSave(ConfigNode node)
        {
            IEndpoint endpoint;
            for (int i = 0; i < _endpoints.Count; i++)
            {
                endpoint = _endpoints[i];
                var endpointNode = new ConfigNode(_endpointNodeName);
                endpointNode.AddValue("Id", endpoint.Id);
                endpointNode.AddValue("Type", endpoint.Type);
            }

            IContract contract;
            for (int i = 0; i < _contracts.Count; i++)
            {
                contract = _contracts[i];
                var contractNode = new ConfigNode(_contractNodeName);
                contractNode.AddValue("ContractId", contract.ContractId);
                contractNode.AddValue("SourceId", contract.Source.Id);
                contractNode.AddValue("DestinationId", contract.Destination.Id);
                contractNode.AddValue("ResourceName", contract.ResourceName);
                contractNode.AddValue("Quantity", contract.Quantity);
                contractNode.AddValue("Rate", contract.Rate);
                contractNode.AddValue("ExpirationTimestamp", contract.ExpirationTimestamp);
                contractNode.AddValue("Priority", contract.Priority);
                contractNode.AddValue("State", contract.State);

                node.AddNode(contractNode);
            }
        }

        public void RemoveEndpoint(string endpointId)
        {
            var endpoint = FindEndpoint(endpointId);

            if (endpoint != null)
            {
                _endpoints.Remove(endpoint);
                endpoint.Dispose();

                ValidateContracts();
            }
        }

        public void ValidateContracts()
        {
            var nonActiveContracts = _contracts.Where(c => c.State != ContractState.Active).ToArray();
            if (nonActiveContracts != null && nonActiveContracts.Length > 0)
            {
                IContract contract;
                for (int i = 0; i < nonActiveContracts.Length; i++)
                {
                    contract = nonActiveContracts[i];
                    var destination = contract.Destination;
                    var resource = contract.ResourceName;

                    var isHonoringContracts = destination.IsHonoringContracts(resource);
                    if (!isHonoringContracts)
                    {
                        PrioritizeContracts(destination, resource);
                    }
                }
            }
        }

        public void ValidateContracts(string endpointId)
        {
            var endpoint = FindEndpoint(endpointId);
            if (endpoint != null)
            {
                var resourcesInBrokenContracts = endpoint.Contracts
                    .Where(c => c.State == ContractState.Broken && c.Source == endpoint)
                    .Select(c => c.ResourceName)
                    .Distinct()
                    .ToArray();

                // This should effectively cover all cases where there has been a change that would cause
                //  a contract to break or be restored, because PrioritizeContracts will end up reactivating
                //  all contracts if the Incoming quantity has increased above the Outgoing quantity since
                //  the last time it was checked.
                if (resourcesInBrokenContracts != null && resourcesInBrokenContracts.Length > 0)
                {
                    string resource;
                    for (int i = 0; i < resourcesInBrokenContracts.Length; i++)
                    {
                        resource = resourcesInBrokenContracts[i];
                        PrioritizeContracts(endpoint, resource);
                    }
                }
            }
        }

        private void LoadContract(IContract contract)
        {
            if (!_contracts.Contains(contract))
                _contracts.Add(contract);
        }

        private void LoadEndpoint(IEndpoint endpoint)
        {
            if (!_endpoints.Contains(endpoint))
                _endpoints.Add(endpoint);
        }

        private void PrioritizeContracts(IEndpoint endpoint, string resourceName)
        {
            var contracts = endpoint.Contracts
                .Where(c => c.ResourceName == resourceName && c.Source == endpoint)
                .OrderByDescending(c => c.Priority)
                .ToArray();

            var available = endpoint.Incoming(resourceName);
            IContract contract;
            for (int i = 0; i < contracts.Length; i++)
            {
                contract = contracts[i];
                var normalizedQuantity = ContractRateConversions.Convert(contract.Quantity, contract.Rate, ContractRateUnit.PerDay);
                if (_validOutgoingContractStates.Contains(contract.State) && available >= normalizedQuantity)
                {
                    contract.State = ContractState.Active;
                    available -= normalizedQuantity;
                }
                else
                {
                    contract.State = ContractState.Broken;
                }
            }
        }
    }
}
