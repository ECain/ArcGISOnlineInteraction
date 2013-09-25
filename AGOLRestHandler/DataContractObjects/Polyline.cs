using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  class Polyline
  {
    [DataMember]
    public PathGeometry[] paths { get; set; }

    [DataMember]
    public SpatialReference spatialReference { get; set; }
  }
}
