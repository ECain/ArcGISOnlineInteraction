using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class Database
  {
    [DataMember]
    public DataSource dataSource { get; set; }
  }
}
