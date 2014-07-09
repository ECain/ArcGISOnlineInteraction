using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  public class Symbol
  {
    [DataMember]
    public string type { get; set; }

    [DataMember]
    public string url { get; set; }

    [DataMember]
    public string imageData { get; set; }

    [DataMember]
    public string contentType { get; set; }

    [DataMember]
    public object color { get; set; }

    [DataMember]
    public double width { get; set; }

    [DataMember]
    public double height { get; set; }

    [DataMember]
    public int angle { get; set; }

    [DataMember]
    public int xoffset { get; set; }

    [DataMember]
    public int yoffset { get; set; }
  }
}
