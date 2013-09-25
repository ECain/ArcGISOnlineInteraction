using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class Context
  {
    [DataMember]
    public Extent extent { get; set; }

    [DataMember]
    public Part[] parts { get; set; } 
  }

  [DataContract]
  public class Part
  {
    [DataMember]
    public Extent extent { get; set; }

    [DataMember]
    public int[] frameids { get; set; }
  }
}
