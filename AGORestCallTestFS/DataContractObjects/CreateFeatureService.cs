using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class CreateFeatureService
  {
    [DataMember]
    public bool success { get; set; }

    [DataMember]
    public Error error { get; set; }
  }
}
