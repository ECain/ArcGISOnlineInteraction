using System.Runtime.Serialization;
namespace AGORestCallTestFS
{
  [DataContract]
  class ServiceCatalog
  {
    [DataMember]
    public string currentVersion { get; set; }

    [DataMember]
    public Service[] services { get; set; }
  }
}
