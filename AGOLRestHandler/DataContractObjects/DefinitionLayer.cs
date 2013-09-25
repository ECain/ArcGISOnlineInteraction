using System.Runtime.Serialization;
//Similar to CreateFeatureServiceObjectParameters, but contains less properties. 
//We cannot include more properties than the server expects or it will error. Hence 
//a near copy.
namespace AGOLRestHandler
{
  [DataContract]
  public class DefinitionLayer
  {
    [DataMember]
    public double currentVersion { get; set; }

    //[DataMember]
    //public string serviceDefinition { get; set; }

    [DataMember]
    public int id { get; set; }

    [DataMember]
    public string name { get; set; }

    [DataMember]
    public string type { get; set; }

    [DataMember]
    public string displayField { get; set; }

    [DataMember]
    public string description { get; set; }

    [DataMember]
    public string copyrightText { get; set; }

    [DataMember]
    public bool defaultVisibility { get; set; }

    [DataMember]
    public object[] relationships { get; set; }

    [DataMember]
    public bool isDataVersioned { get; set; }

    [DataMember]
    public bool supportsRollbackOnFailureParameter { get; set; }

    [DataMember]
    public bool supportsStatistics { get; set; }

    [DataMember]
    public bool supportsAdvancedQueries { get; set; }

    [DataMember]
    public string geometryType { get; set; }

    [DataMember]
    public int minScale { get; set; }

    [DataMember]
    public int maxScale { get; set; }

    [DataMember]
    public Extent extent { get; set; }

    [DataMember]
    public /*DrawingInfo*/string drawingInfo { get; set; }

    [DataMember]
    public bool allowGeometryUpdates { get; set; }

    [DataMember]
    public bool hasAttachments { get; set; }

    [DataMember]
    public string htmlPopupType { get; set; }

    [DataMember]
    public bool hasM { get; set; }

    [DataMember]
    public bool hasZ { get; set; }

    [DataMember]
    public string objectIdField { get; set; }

    [DataMember]
    public string globalIdField { get; set; }

    [DataMember]
    public string typeIdField { get; set; }

    [DataMember]
    public object[] fields { get; set; }

    [DataMember]
    public object[] types { get; set; }

    [DataMember]
    public Template[] templates { get; set; }

    [DataMember]
    public string supportedQueryFormats { get; set; }

    [DataMember]
    public bool hasStaticData { get; set; }

    [DataMember]
    public int maxRecordCount { get; set; }

    [DataMember]
    public string capabilities { get; set; }

    //[DataMember]
    //public EditorTrackingInfo editorTrackingInfo { get; set; }

    [DataMember]
    public AdminLayerInfoAttribute adminLayerInfo { get; set; }
  }
}
