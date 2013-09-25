using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class XssPreventionInfo
  {
    [DataMember]
    public bool xssPreventionEnabled { get; set; }

    [DataMember]
    public string xssPreventionRule { get; set; }

    [DataMember]
    public string xssInputRule { get; set; }
  }
}
