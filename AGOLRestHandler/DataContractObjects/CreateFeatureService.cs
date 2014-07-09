using System.Runtime.Serialization;

namespace AGOLRestHandler
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
