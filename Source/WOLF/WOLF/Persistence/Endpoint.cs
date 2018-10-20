namespace Quartermaster
{
    public class Endpoint
    {
        public string EndpointId { get; set; }
        public string VesselId { get; set; }
        public int MainBodyIndex { get; set; }
        public bool IsLanded { get; set; }
        public EndpointTypes Type { get; set; }
    }

    public enum EndpointTypes
    {
        Pool,
        Transport,
        Converter,
        Hopper
    }
}