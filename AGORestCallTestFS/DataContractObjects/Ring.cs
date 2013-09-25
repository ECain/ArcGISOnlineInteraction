using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class RingGeometry
  {
    [DataMember]
    public GeometryPoint[] ring { get; set; }
  }
}
