using System;
using System.Collections.Generic;
using System.Linq;

namespace WOLF.Tests.Unit.Mocks
{
    public class MockNegotiator : IContractNegotiator
    {
        public List<IEndpoint> Endpoints = new List<IEndpoint>();
        public List<IContract> Contracts = new List<IContract>();

        public string AddEndpoint(IEndpoint endpoint)
        {
            endpoint.Id = Guid.NewGuid().ToString();
            Endpoints.Add(endpoint);

            return endpoint.Id;
        }

        public IContract FindContract(string contractId)
        {
            return Contracts.Where(c => c.ContractId == contractId).FirstOrDefault();
        }

        public IEndpoint FindEndpoint(string endpointId)
        {
            return Endpoints.Where(e => e.Id == endpointId).FirstOrDefault();
        }

        public void LoadContract(IContract contract)
        {
            if (!Contracts.Contains(contract))
                Contracts.Add(contract);
        }

        public void LoadEndpoint(IEndpoint endpoint)
        {
            if (!Endpoints.Contains(endpoint))
                Endpoints.Add(endpoint);

            // Add any unregistered contracts
            if (endpoint.Contracts != null && endpoint.Contracts.Count > 0)
            {
                foreach (var contract in endpoint.Contracts)
                {
                    LoadContract(contract);
                }
            }

            // Make sure endpoint has references to all its contracts
            var contracts = Contracts.Where(c => c.Source == endpoint || c.Destination == endpoint);
            foreach (var contract in contracts)
            {
                if (!endpoint.Contracts.Contains(contract))
                    endpoint.Contracts.Add(contract);
            }
        }

        public NegotiationResult Negotiate(string sourceId, string destinationId, string resourceName, double quantity, ContractRateUnit rate, double expiration = 0d)
        {
            var source = Endpoints.Where(e => e.Id == sourceId).FirstOrDefault();
            var destination = Endpoints.Where(e => e.Id == destinationId).FirstOrDefault();
            var contract = new Contract
            {
                ContractId = Guid.NewGuid().ToString(),
                Source = source,
                Destination = destination,
                ResourceName = resourceName,
                Quantity = quantity,
                Rate = rate
            };

            return new OkNegotiationResult<Contract>(contract);
        }

        public void RemoveEndpoint(string endpointId)
        {
            var endpoint = Endpoints.Where(e => e.Id == endpointId).FirstOrDefault();
            Endpoints.Remove(endpoint);
        }

        public void ValidateContracts()
        {
            throw new NotImplementedException();
        }
    }
}
