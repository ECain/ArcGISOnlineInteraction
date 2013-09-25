using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract] //TODO: CODE CHANGES REQUIRED TO MAKE THIS WORK FOR POINTS. 
  public class Symbol
  {
    [DataMember]
    public string type { get; set; }

    [DataMember]
    public string style { get; set; }

    [DataMember]
    public object color { get; set; }

    [DataMember]
    public Outline outline { get; set; }

    [DataMember]
    public string imageData { get; set; }

    [DataMember]
    public double width { get; set; }
  }

  [DataContract]
  public class PointSymbol : Symbol
  {
    [DataMember]
    public string url { get; set; }

    [DataMember]
    public string contentType { get; set; }

    [DataMember]
    public double height { get; set; }

    [DataMember]
    public int angle { get; set; }

    [DataMember]
    public int xoffset { get; set; }

    [DataMember]
    public int yoffset { get; set; }
  }

  [DataContract]
  public class PolygonSymbol : Symbol
  {
    [DataMember]
    public Outline outline { get; set; }
  }
}
