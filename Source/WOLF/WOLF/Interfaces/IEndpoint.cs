using System;
using System.Collections.Generic;

namespace WOLF
{
    public interface IEndpoint : IDisposable, IPersistenceAware
    {
        string Id { get; set; }
        List<IContract> Contracts { get; }
        string Type { get; }

        bool CanProvide(string resourceName, double quantity, ContractRateUnit rate);
        bool IsHonoringContracts(string resourceName);
        double Incoming(string resourceName, ContractRateUnit rate = ContractRateUnit.PerDay);
        double Outgoing(string resourceName, ContractRateUnit rate = ContractRateUnit.PerDay);
        double Free(string resourceName, ContractRateUnit rate = ContractRateUnit.PerDay);
    }
}
