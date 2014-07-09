using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class FeatureServiceNameAvailability
  {
    [DataMember]
    public bool available { get; set; }
  }
}
