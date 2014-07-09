using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class GeometryResponse
  {
    [DataMember]
    public string displayFieldName { get; set; }

    [DataMember]
    public object fieldAliases { get; set; }

    [DataMember]
    public Field[] fields { get; set; }

    [DataMember]
    public Feature[] features { get; set; }
  }
}
