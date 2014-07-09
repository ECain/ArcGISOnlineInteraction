using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  class MultiPoint
  {
    [DataMember]
    public GeometryPoint[] points { get; set; }

    [DataMember]
    public SpatialReference spatialReference { get; set; }
  }
}
