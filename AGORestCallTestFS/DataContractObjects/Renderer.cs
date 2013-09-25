using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  public class Renderer
  {
    [DataMember]
    public string type { get; set; }

    [DataMember]
    public Symbol symbol { get; set; }

    [DataMember]
    public string label { get; set; }

    [DataMember]
    public string description { get; set; }
  }
}
