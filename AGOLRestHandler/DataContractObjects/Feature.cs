using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class Feature
  {
    [DataMember]
    public object attributes { get; set; }
    //todo
    [DataMember]
    public object[] geometry { get; set; }
  }
}
