using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class Service
  {
    [DataMember]
    public string name { get; set; }

    [DataMember]
    public string type { get; set; }
  }
}
