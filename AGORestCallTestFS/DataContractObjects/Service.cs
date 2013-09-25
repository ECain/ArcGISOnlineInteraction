using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class Service
  {
    [DataMember]
    public string name { get; set; }

    [DataMember]
    public string type { get; set; }
  }
}
