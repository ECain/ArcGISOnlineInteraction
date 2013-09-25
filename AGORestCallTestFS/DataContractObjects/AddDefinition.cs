using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class AddDefinition
  {
    [DataMember]
    public DefinitionLayer[] layers { get; set; }
  }
}
