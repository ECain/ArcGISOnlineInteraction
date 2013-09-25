using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class AttributePushSuccess
  {
    [DataMember]
    public bool success { get; set; }
  }
}
