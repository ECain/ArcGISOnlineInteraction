using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class Administration
  {
    [DataMember]
    public string currentVersion { get; set; }

    [DataMember]
    public string[] resources { get; set; }

    [DataMember]
    public string serverType { get; set; }
  }
}
