using System;

namespace Quartermaster
{
    public class ResourceLink
    {
        public string LinkId { get; set; }
        public string SourceId { get; set; }
        public string DestinationId { get; set; }
        public string ResourceName { get; set; }
        public int Quantity { get; set; }
    }
}