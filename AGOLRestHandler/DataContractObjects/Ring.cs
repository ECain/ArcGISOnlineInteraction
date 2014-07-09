using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  class RingGeometry
  {
    [DataMember]
    public GeometryPoint[] ring { get; set; }
  }
}
