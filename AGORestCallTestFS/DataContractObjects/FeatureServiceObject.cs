using System.Runtime.Serialization;
//Help: http://services.arcgis.com/help/index.html?applyedits.html

namespace AGORestCallTestFS
{
  [DataContract]
  class FeatureServiceObject
  {
    [DataMember]
    public string name { get; set; }

    [DataMember]
    public string type { get; set; }

    [DataMember]
    public string status { get; set; }

    [DataMember]
    public Database database { get; set; }

    [DataMember]
    public string capabilities { get; set; }

    [DataMember]
    public ConnectionAttributes connectionAttributes { get; set; }

    [DataMember]
    public int maxRecordCount { get; set; }
  }
}
