namespace WOLF
{
    public enum ContractRateUnit { PerSecond, PerMinute, PerHour, PerDay }
    public enum ContractState { Active, Broken, Disposed, Expired, Inactive }

    public interface IContract
    {
        string ContractId { get; set; }
        IEndpoint Source { get; set; }
        IEndpoint Destination { get; set; }
        string ResourceName { get; set; }
        double Quantity { get; set; }
        ContractRateUnit Rate { get; set; }
        ContractState State { get; set; }
        double ExpirationTimestamp { get; set; }
        int Priority { get; set; }
    }
}
