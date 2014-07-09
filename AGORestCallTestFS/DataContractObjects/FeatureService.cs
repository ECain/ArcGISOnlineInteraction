using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class FeatureService
  {
    [DataMember]
    public string serviceDescription { get; set; }

    [DataMember]
    public bool supportsRollBackOnFailures { get; set; }

    [DataMember]
    public Layer[] layers { get; set; }

    [DataMember]
    public Table[] tables { get; set; }
  }
}
