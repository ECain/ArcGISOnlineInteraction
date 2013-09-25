using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class DrawingInfo
  {
    [DataMember]
    public Renderer renderer {get; set;}

    [DataMember]
    public LabelingInformation[] labelingInfo {get; set;}
  }
}
