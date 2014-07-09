using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  public class SpatialReference
  {
    [DataMember]
    public long wkid { get; set; }

    //[DataMember]
    //public long latestWkid { get; set; }
  }
}
