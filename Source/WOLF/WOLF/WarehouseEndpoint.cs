using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOLF
{
    /// <summary>
    /// A hub for contracts that move resources from point to point.
    /// </summary>
    public class WarehouseEndpoint : AbstractResourceNetworkEndpoint
    {
        public WarehouseEndpoint() : base("Warehouse")
        {
        }
    }
}
