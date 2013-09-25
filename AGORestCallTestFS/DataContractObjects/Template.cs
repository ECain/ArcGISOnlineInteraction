using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  public class Template
  {
    [DataMember]
    public string name { get; set; }

    [DataMember]
    public string description { get; set; }

    [DataMember]
    public string drawingTool { get; set; }

    [DataMember]
    public Prototype prototype { get; set; }
  }
}
