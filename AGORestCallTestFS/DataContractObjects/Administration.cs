using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class Administration
  {
    [DataMember]
    public string currentVersion { get; set; }

    [DataMember]
    public string[] resources { get; set; }

    [DataMember]
    public string serverType { get; set; }
  }
}
