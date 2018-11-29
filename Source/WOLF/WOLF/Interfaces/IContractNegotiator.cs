namespace WOLF
{
    public interface IContractNegotiator
    {
        NegotiationResult Negotiate(IRecipe recipe);
    }
}
