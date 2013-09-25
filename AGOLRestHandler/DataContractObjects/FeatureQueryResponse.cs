using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class FeatureQueryResponse
  {
    [DataMember]
    public string objectIdFieldName { get; set; }

    [DataMember]
    public string globalIdFieldName { get; set; }

    [DataMember]
    public string geometryType { get; set; }

    [DataMember]
    public SpatialReference spatialReference { get; set; }

    [DataMember]
    public object[] fields { get; set; }

    [DataMember]
    public object[] features { get; set; }
  }
}
