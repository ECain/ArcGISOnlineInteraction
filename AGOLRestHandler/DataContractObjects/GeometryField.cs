using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class GeometryField
  {
    [DataMember]
    public string name { get; set; }

    [DataMember]
    public long srid { get; set; }
  }
}
