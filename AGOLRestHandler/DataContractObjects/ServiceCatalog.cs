using System.Runtime.Serialization;
namespace AGOLRestHandler
{
  [DataContract]
  public class ServiceCatalog
  {
    [DataMember]
    public string currentVersion { get; set; }

    [DataMember]
    public Service[] services { get; set; }
  }
}
