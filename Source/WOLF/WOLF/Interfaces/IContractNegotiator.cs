namespace WOLF
{
    public interface IContractNegotiator : IPersistenceAware
    {
        string AddEndpoint(IEndpoint endpoint);
        IContract FindContract(string contractId);
        IEndpoint FindEndpoint(string endpointId);
        NegotiationResult Negotiate(string sourceId, string destinationId, string resourceName, double quantity, ContractRateUnit rate, double expiration = 0d);
        void RemoveEndpoint(string endpointId);
        void ValidateContracts();
    }
}
