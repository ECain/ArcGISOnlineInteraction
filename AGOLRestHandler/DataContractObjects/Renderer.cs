using System.Runtime.Serialization;

namespace AGOLRestHandler
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

  [DataContract]
  public class PointRenderer : Renderer
  {
    [DataMember]
    public string field1 { get; set; }

    [DataMember]
    public Symbol defaultSymbol { get; set; }

    //[DataMember]
    //public string defaultLabel { get; set; }

    [DataMember]
    public UniqueValueInfo[] uniqueValueInfos { get; set; }
  }

  [DataContract]
  public class PolygonRenderer : Renderer
  {
    [DataMember]
    public string field1 { get; set; }

    [DataMember]
    public string field2 { get; set; }

    [DataMember]
    public string field3 { get; set; }

    [DataMember]
    public string fieldDelimiter { get; set; }

    [DataMember]
    public Symbol defaultSymbol { get; set; }

    [DataMember]
    public string defaultLabel { get; set; }

    [DataMember]
    public UniqueValueInfo[] uniqueValueInfos { get; set; }
  }
}
