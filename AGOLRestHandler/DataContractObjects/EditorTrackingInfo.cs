using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class EditorTrackingInfo
  {
    [DataMember]
    public bool enableEditorTracking { get; set; }

    [DataMember]
    public bool enableOwnershipAccessControl { get; set; }

    [DataMember]
    public bool allowOthersToUpdate { get; set; }

    [DataMember]
    public bool allowOthersToDelete { get; set; }
  }
}
