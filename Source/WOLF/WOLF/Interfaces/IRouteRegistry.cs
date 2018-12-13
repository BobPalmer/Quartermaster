namespace WOLF
{
    public interface IRouteRegistry
    {
        IRoute CreateRoute(
            string originBody,
            string originBiome,
            string destinationBody,
            string destinationBiome,
            int payload);
        IRoute GetRoute(
            string originBody,
            string originBiome,
            string destinationBody,
            string destinationBiome);
        bool HasRoute(
            string originBody,
            string originBiome,
            string destinationBody,
            string destinationBiome);
    }
}
