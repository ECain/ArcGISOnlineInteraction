using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class SpatialReference
  {
    [DataMember]
    public long wkid { get; set; }

    [DataMember]
    public long latestWkid { get; set; }
  }
}
