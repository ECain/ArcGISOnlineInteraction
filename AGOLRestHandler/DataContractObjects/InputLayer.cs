using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class InputLayer
  {
    [DataMember]
    public string url { get; set; }

    [DataMember]
    public string serviceToken { get; set; }           
  }
}
