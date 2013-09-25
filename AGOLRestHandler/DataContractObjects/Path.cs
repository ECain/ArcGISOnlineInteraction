using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  class PathGeometry
  {
    [DataMember]
    public GeometryPoint[] path { get; set; }
  }
}