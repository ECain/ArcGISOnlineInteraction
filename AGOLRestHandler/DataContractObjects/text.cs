using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class Text
  {
    [DataMember]
    public UpdateLayer[] layers { get; set; }

    [DataMember]
    public AnalysisInfo analysisInfo { get; set; }
  }

  [DataContract]
  public class UpdateLayer
  {
    [DataMember]
    public int id { get; set; }

    public PopupInfo popupInfo { get; set; }
  }

  [DataContract]
  public class PopupInfo
  {
    [DataMember]
    public bool showAttachments { get; set; }

    [DataMember]
    public FieldInfo[] fieldInfos { get; set; }

    [DataMember]
    public string description { get; set; }

    [DataMember]
    public string label { get; set; }
  }

  [DataContract]
  public class FieldInfo
  {
    [DataMember]
    public bool isEditable { get; set; }

    [DataMember]
    public string stringFieldOption { get; set; }

    [DataMember]
    public string tooltip { get; set; }

    [DataMember]
    public string label { get; set; }

    [DataMember]
    public bool visible { get; set; }

    [DataMember]
    public string fieldname { get; set; }

  }
}