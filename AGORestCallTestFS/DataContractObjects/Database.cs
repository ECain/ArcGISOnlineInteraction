using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class Database
  {
    [DataMember]
    public DataSource dataSource { get; set; }
  }
}
