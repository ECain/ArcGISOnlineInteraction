using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class AdminLayerInfo
  {
    [DataMember]
    public string TableName { get; set; }

    [DataMember]
    public GeometryField geometryField { get; set; }

    [DataMember]
    public Extent TableExtent { get; set; }
  }

  //Used for AddDefinition on Feature Services
  [DataContract]
  public class AdminLayerInfoAttribute
  {
    [DataMember]
    public GeometryField geometryField { get; set; }
  }
}
