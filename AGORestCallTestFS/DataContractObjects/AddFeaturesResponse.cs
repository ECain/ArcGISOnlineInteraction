using System.Runtime.Serialization;

namespace AGORestCallTestFS
{
  [DataContract]
  class FeatureEditsResponse
  {
    [DataMember]
    public Result[] addResults { get; set; }

    [DataMember]
    public Result[] updateResults { get; set; }

    [DataMember]
    public Result[] deleteResults { get; set; }
  }

  [DataContract]
  class Result
  {
    [DataMember]
    public int objectId { get; set; }

    [DataMember]
    public object globalId { get; set; }

    [DataMember]
    public bool success { get; set; }

    [DataMember]
    public Error error { get; set; }
  }

  [DataContract]
  class Error
  {
    [DataMember]
    public int code { get; set; }

    [DataMember]
    public string description { get; set; }

    [DataMember]
    public string message { get; set; }

    [DataMember]
    public string[] details { get; set; }
  }
}
