using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class Feature
  {
    [DataMember]
    public object[] attributes { get; set; }
    //todo
    [DataMember]
    public object[] geometry { get; set; }
  }
}
