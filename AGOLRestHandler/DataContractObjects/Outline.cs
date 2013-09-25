using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class Outline
  {
    [DataMember]
    public string type { get; set; }

    [DataMember]
    public string style { get; set; }

    [DataMember]
    public object color { get; set; }

    [DataMember]
    public double width { get; set; }
  }
}
