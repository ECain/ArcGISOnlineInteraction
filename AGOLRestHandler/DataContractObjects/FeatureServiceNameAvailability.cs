using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  class FeatureServiceNameAvailability
  {
    [DataMember]
    public bool available { get; set; }
  }
}
