using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  class GenerateToken
  {
    [DataMember]
    public string userName { get; set; }

    [DataMember]
    public string password { get; set; }

    [DataMember]
    public string client { get; set; }

    [DataMember]
    public string referer { get; set; }

    [DataMember]
    public int expiration { get; set; }
  }
}
