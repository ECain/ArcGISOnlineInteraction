using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class DrawingInfo
  {
    [DataMember]
    public Renderer renderer {get; set;}
    //public string renderer { get; set; }

    [DataMember]
    public int transparency { get; set; }

    [DataMember]
    public LabelingInformation[] labelingInfo {get; set;}
  }
}
