using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class FeatureServiceInfo
  {
    [DataMember]
    public FeatureServiceObject adminServiceInfo { get; set; }

    [DataMember]
    public bool allowGeometryUpdates { get; set; }

    [DataMember]
    public double currentVersion { get; set; }

    [DataMember]
    public bool hasVersionedData { get; set; }

    [DataMember]
    public bool hasStaticData { get; set; }

    [DataMember]
    public EditorTrackingInfo editorTrackingInfo { get; set; }

    [DataMember]
    public XssPreventionInfo xssPreventionInfo { get; set; }

    [DataMember]
    public string serviceDescription { get; set; }

    [DataMember]
    public bool supportsRollbackOnFailure { get; set; }

    [DataMember]
    public bool supportsDisconnectedEditing { get; set; }

    [DataMember]
    public int maxRecordCount { get; set; }

    [DataMember]
    public string supportedQueryFormats { get; set; }

    [DataMember]
    public string capabilities { get; set; }

    [DataMember]
    public string description { get; set; }

    [DataMember]
    public string copyrightText { get; set; }

    [DataMember]
    public SpatialReference spatialReference { get; set; }

    [DataMember]
    public Extent initialExtent { get; set; }

    [DataMember]
    public Extent fullExtent { get; set; }

    [DataMember]
    public string units { get; set; }

    [DataMember]
    public Layer[] layers { get; set; }

    [DataMember]
    public Table[] tables { get; set; }

    [DataMember]
    public string name { get; set; }
  }
}
