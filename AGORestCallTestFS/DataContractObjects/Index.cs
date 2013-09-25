using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  public class Index
  {
    [DataMember]
    public string name { get; set; }

    [DataMember]
    public string fields { get; set; }

    [DataMember]
    public bool isAscending { get; set; }

    [DataMember]
    public bool isUnique { get; set; }

    [DataMember]
    public string description { get; set; }
  }
}
