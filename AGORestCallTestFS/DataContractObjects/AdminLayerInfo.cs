using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class AdminLayerInfo
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
  class AdminLayerInfoAttribute
  {
    [DataMember]
    public GeometryField geometryField { get; set; }
  }
}
