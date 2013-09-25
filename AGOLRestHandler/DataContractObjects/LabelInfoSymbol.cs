using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class LabelInfoSymbol
  {
    [DataMember]
    public string type { get; set; }

    [DataMember]
    public object[] color { get; set; }

    [DataMember]
    public object[] backgroundColor { get; set; }

    [DataMember]
    public object[] borderLineColor { get; set; }

    [DataMember]
    public string verticalAlignment { get; set; }

    [DataMember]
    public string horizontalAlignment { get; set; }

    [DataMember]
    public bool rightToLeft { get; set; }

    [DataMember]
    public int angle { get; set; }

    [DataMember]
    public int xoffset { get; set; }

    [DataMember]
    public int yoffset { get; set; }

    [DataMember]
    public object font { get; set;}
  }
}
