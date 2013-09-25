using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class Table
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }
  }
}
