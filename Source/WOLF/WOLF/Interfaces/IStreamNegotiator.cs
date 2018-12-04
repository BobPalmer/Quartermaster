namespace WOLF
{
    public interface IStreamNegotiator
    {
        NegotiationResult Negotiate(IRecipe recipe);
    }
}
