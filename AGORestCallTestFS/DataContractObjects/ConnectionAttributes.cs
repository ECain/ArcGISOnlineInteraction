using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class ConnectionAttributes
  {
    [DataMember]
    public string serverName { get; set; }

    [DataMember]
    public string databaseName { get; set; }

    [DataMember]
    public string userName { get; set; }

    [DataMember]
    public string password { get; set; }
  }
}
