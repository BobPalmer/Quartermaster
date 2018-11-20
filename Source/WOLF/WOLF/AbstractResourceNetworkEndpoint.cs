using System;
using System.Collections.Generic;
using System.Linq;

namespace WOLF
{
    public abstract class AbstractResourceNetworkEndpoint : IEndpoint
    {
        public string Id { get; set; }
        public List<IContract> Contracts { get; private set; } = new List<IContract>();
        public string Type { get; private set; }

        private ContractState[] _validOutgoingStates = new ContractState[] { ContractState.Active, ContractState.Broken };

        public AbstractResourceNetworkEndpoint(string endpointType)
        {
            Type = endpointType;
        }

        public virtual bool CanProvide(string resourceName, double quantity, ContractRateUnit rate)
        {
            return Free(resourceName, rate) >= quantity;
        }

        public virtual bool IsHonoringContracts(string resourceName)
        {
            return Incoming(resourceName) >= Outgoing(resourceName);
        }

        public virtual double Incoming(string resourceName, ContractRateUnit rate = ContractRateUnit.PerDay)
        {
            return Contracts
                .Where(c => c.ResourceName == resourceName && c.Destination == this && c.State == ContractState.Active)
                .Sum(c => ContractRateConversions.Convert(c.Quantity, c.Rate, rate));
        }

        public virtual double Outgoing(string resourceName, ContractRateUnit rate = ContractRateUnit.PerDay)
        {
            return Contracts
                .Where(c => c.ResourceName == resourceName && c.Source == this && _validOutgoingStates.Contains(c.State))
                .Sum(c => ContractRateConversions.Convert(c.Quantity, c.Rate, rate));
        }

        public virtual double Free(string resourceName, ContractRateUnit rate)
        {
            var incoming = Incoming(resourceName, rate);
            var outgoing = Outgoing(resourceName, rate);

            return Math.Max(0d, incoming - outgoing);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;

                if (disposing)
                {
                    for (int i = 0; i < Contracts.Count; i++)
                    {
                        var contract = Contracts[i];
                        contract.State = ContractState.Disposed;
                    }

                    Contracts = null;
                }
            }
        }

        /// <summary>
        /// This should not be called manually. Use <see cref="IContractNegotiator.RemoveEndpoint(string)"/> instead.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
