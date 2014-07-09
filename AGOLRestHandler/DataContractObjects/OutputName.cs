using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class OutputName
  {
    [DataMember]
    public ServiceProperty serviceProperties { get; set; }

    [DataMember]
    public ItemProperties itemProperties { get; set; }
  }

  [DataContract]
  public class ServiceProperty
  {
    [DataMember]
    public string name {get; set;}
  }

  [DataContract]
  public class ItemProperties
  {
    [DataMember]
    public string itemID { get; set; }
  }
}
