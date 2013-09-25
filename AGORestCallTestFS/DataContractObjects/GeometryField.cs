using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class GeometryField
  {
    [DataMember]
    public string name { get; set; }

    [DataMember]
    public long srid { get; set; }
  }
}
