using System;
using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class Self
  {
    [DataMember]
    public string username {get; set;}

    [DataMember]
    public string fullName {get; set;}

    [DataMember]
    public string preferredView {get; set;}

    [DataMember]
    public string description {get; set;}

    [DataMember]
    public string email {get; set;}

    [DataMember]
    public string access {get; set;}

    [DataMember]
    public ulong storageUsage { get; set; }

    [DataMember]
    public ulong storageQuota { get; set; }

    [DataMember]
    public string orgId {get; set;}

    [DataMember]
    public string role {get; set;}

    [DataMember]
    public object[] tags {get; set;} 

    [DataMember]
    public string culture {get; set;}

    [DataMember]
    public string region {get; set;}

    [DataMember]
    public string thumbnail {get; set;}

    [DataMember]
    public ulong created { get; set; } 

    [DataMember]
    public ulong modified { get; set; }

    [DataMember]
    public Group[] groups {get; set;}
  }

  [DataContract]
  public class Group
  {
    [DataMember]
    public string id { get; set; }

    [DataMember]
    public string title {get; set;}

    [DataMember]
    public bool isInvitationOnly {get; set;}    

    [DataMember]
    public string owner {get; set;}     

    [DataMember]
    public string description {get; set;}

    [DataMember]
    public string snippet {get; set;}

    [DataMember]
    public string[] tags {get; set;}

    [DataMember]
    public string phone { get; set; }     

    [DataMember]
    public string thumbnail {get; set;}

    [DataMember]
    public ulong created { get; set; }  

    [DataMember]
    public ulong modified { get; set; } 

    [DataMember]
    public string access {get; set;}    

    [DataMember]
    public UserMembership userMembership {get; set;}      
  }

  public class UserMembership
  {
    [DataMember]
    public string username {get; set;}

    [DataMember]
    public string memberType {get; set;}

    [DataMember]
    public int applications {get; set;}
  }
}
