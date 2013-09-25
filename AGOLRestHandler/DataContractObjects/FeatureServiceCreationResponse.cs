using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class FeatureServiceCreationResponse
  {
    [DataMember]
    public string EncodedServiceURL { get; set; }

    [DataMember]
    public string ItemId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string ServiceItemId { get; set; }

    [DataMember]
    public string ServiceUrl { get; set; }

    [DataMember]
    public int Size { get; set; }

    [DataMember]
    public bool Success { get; set; }

    [DataMember]
    public string Type { get; set; }
  }
}
