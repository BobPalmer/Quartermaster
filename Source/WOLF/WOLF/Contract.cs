namespace WOLF
{
    public class Contract : IContract
    {
        public string ContractId { get; set; }
        public IEndpoint Source { get; set; }
        public IEndpoint Destination { get; set; }
        public string ResourceName { get; set; }
        public double Quantity { get; set; }
        public ContractRateUnit Rate { get; set; } = ContractRateUnit.PerDay;
        public ContractState State { get; set; } = ContractState.Active;
        public double ExpirationTimestamp { get; set; } = 0d;
        public int Priority { get; set; }
    }
}
