using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class AddDefinition
  {
    [DataMember]
    public DefinitionLayer[] layers { get; set; }
  }
}
