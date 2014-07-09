using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  public class Extent
  {
    [DataMember]
    public double xmin { get; set; }

    [DataMember]
    public double ymin { get; set; }

    [DataMember]
    public double xmax { get; set; }

    [DataMember]
    public double ymax { get; set; }

    [DataMember]
    public SpatialReference spatialReference { get; set; }
  }
}
