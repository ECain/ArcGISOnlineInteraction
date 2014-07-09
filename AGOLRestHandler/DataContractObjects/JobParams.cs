using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class JobParams
  {
    [DataMember]
    public InputLayer InputLayer { get; set; }
    //public string InputLayer { get; set; }

    [DataMember]
    public string DissolveType { get; set; }

    [DataMember]
    public int[] Distances { get; set; }

    [DataMember]
    public string Units { get; set; } 

    [DataMember]
    public string RingType { get; set; } 

    [DataMember]
    public OutputName OutputName { get; set; }
    //public string OutputName { get; set; }

    [DataMember]
    public Context context { get; set; }
    //public string context { get; set; }
      
  }
}
