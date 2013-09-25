using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class EditorTrackingInfo
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
