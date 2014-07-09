using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class LabelingInformation
  {
    [DataMember]
    public string labelPlacement {get; set;}
    
    [DataMember]
    public string labelExpression {get; set;}
 
    [DataMember]
    public bool useCodedValues {get; set;} 
    
    [DataMember]
    public LabelInfoSymbol symbol { get; set; } 

    [DataMember]
    public int minScale {get; set;} 
        
    [DataMember]
    public int maxScale {get; set;} 
  }
}
