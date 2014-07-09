using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class PathGeometry
  {
    [DataMember]
    public GeometryPoint[] path { get; set; }
  }
}