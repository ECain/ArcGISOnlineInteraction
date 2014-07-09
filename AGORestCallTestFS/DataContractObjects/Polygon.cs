using System.Runtime.Serialization;

namespace AGORestCallTestFS
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
