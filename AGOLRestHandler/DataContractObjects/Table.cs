using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class Table
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }
  }
}
