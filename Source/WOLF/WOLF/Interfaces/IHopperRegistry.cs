using System.Collections.Generic;

namespace WOLF
{
    public interface IHopperRegistry
    {
        string CreateHopper(IDepot depot, IRecipe recipe);
        List<HopperMetadata> GetHoppers();
        void RemoveHopper(string id);
    }
}
