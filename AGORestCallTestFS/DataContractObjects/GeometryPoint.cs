using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class GeometryPoint
  {
    [DataMember]
    public double x { get; set; }

    [DataMember]
    public double y { get; set; }
  }
}
