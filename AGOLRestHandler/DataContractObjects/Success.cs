using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  class AttributePushSuccess
  {
    [DataMember]
    public bool success { get; set; }
  }
}
