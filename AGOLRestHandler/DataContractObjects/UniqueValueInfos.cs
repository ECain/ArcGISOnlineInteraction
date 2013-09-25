using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class UniqueValueInfos
  {
    public UniqueValueInfo[] uniqueValueInfos { get; set; }
  }
}
