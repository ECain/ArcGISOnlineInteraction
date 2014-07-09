using System.Runtime.Serialization;

namespace AGORestCallTestFS
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
