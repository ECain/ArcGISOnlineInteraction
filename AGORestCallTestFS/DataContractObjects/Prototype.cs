using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  public class Prototype
  {
    [DataMember]
    public Attributes attributes { get; set; }
  }
}
