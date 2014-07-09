using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class AnalysisInfo
  {
    [DataMember]
    public string toolName { get; set; }

    public JobParams jobParams {get; set; }
  }
}
