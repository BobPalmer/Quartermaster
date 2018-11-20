namespace WOLF
{
    public abstract class NegotiationResult { }

    public class FailedNegotiationResult : NegotiationResult
    {
        public string Reason { get; private set; }

        public FailedNegotiationResult(string reason)
        {
            Reason = reason;
        }
    }

    public class OkNegotiationResult<T> : NegotiationResult
        where T: IContract
    {
        public T Contract { get; private set; }

        public OkNegotiationResult(T contract)
        {
            Contract = contract;
        }
    }
}
