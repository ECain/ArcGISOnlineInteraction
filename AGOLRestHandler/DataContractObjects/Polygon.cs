using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  class Polygon
  {
    [DataMember]
    public RingGeometry[] rings { get; set; }

    [DataMember]
    public SpatialReference spatialReference { get; set; }
  }
}
