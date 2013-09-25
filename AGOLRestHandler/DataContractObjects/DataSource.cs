using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class DataSource
  {
    [DataMember]
    public string name { get; set; }

    [DataMember]
    public string userName { get; set; }

    [DataMember]
    public string password { get; set; }
  }
}
