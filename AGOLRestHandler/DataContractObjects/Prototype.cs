using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class Prototype
  {
    [DataMember]
    public Attributes attributes { get; set; }
  }
}
