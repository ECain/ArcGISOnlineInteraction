using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class UniqueValueInfo
  {
    [DataMember]
    public string value { get; set; }

    [DataMember]
    public string label { get; set; }

    [DataMember]
    public string description { get; set; }

    [DataMember]
    public Symbol symbol { get; set; }
  }
}
