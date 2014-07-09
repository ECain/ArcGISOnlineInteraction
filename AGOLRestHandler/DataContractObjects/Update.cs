using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class Update
  {
    [DataMember]
    public string title { get; set; }

    [DataMember]
    public string description { get; set; }

    [DataMember]
    public string tags { get; set; }

    [DataMember]
    public Extent extent { get; set; }

    [DataMember]
    public string thumbnailURL { get; set; }

    [DataMember]
    public string typeKeywords { get; set; }

    [DataMember]
    public Text text { get; set; }

    //[DataMember]
    //public AnalysisInfo analysisInfo { get; set; }

  }
}
