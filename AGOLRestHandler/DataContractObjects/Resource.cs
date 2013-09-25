using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  class Resource
  {
    [DataMember]
    public string[] dataSources { get; set; }

    [DataMember]
    public Service[] services { get; set; }
  }
}
