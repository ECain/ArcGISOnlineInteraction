using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AGOLRestHandler
{
  [DataContract]
  public class UserOrganizationContent
  {
    [DataMember]
    public string username {get; set;}

    [DataMember]
    public Folder currentFolder  {get; set;}

    [DataMember]
    public List<Item> items {get; set;}

    [DataMember]
    public List<Folder> folders {get; set;}
  }

  [DataContract]
  public class Folder
  {
    [DataMember]
    public string username {get; set;}
    
    [DataMember]
    public string id {get; set;}
    
    [DataMember]
    public string title {get; set;}

    [DataMember]
    public object created {get; set;}
  }

  [DataContract]
  public class Item
  {
    [DataMember]
    public string username {get; set;}

    [DataMember]
    public string id { get; set; }
    
    [DataMember]
    public string item {get; set;} 
    
    [DataMember]
    public string itemType {get; set;}
    
    [DataMember]
    public string owner {get; set;} 

    [DataMember]
    public object uploaded {get; set;} 
    
    [DataMember]
    public object modified {get; set;}
    
    [DataMember]
    public object guid {get; set;}
    
    [DataMember]
    public string name {get; set;}
    
    [DataMember]
    public string title {get; set;}
    
    [DataMember]
    public string type {get; set;}
    
    [DataMember]
    public string[] typeKeywords {get; set;}
    
    [DataMember]
    public string description {get; set;}
    
    [DataMember]
    public string[] tags {get; set;}
    
    [DataMember]
    public object snippet {get; set;}
    
    [DataMember]
    public object thumbnail {get; set;}
    
    [DataMember]
    public object documentation {get; set;}
    
    [DataMember]
    public object[] extent {get; set;}
    
    [DataMember]
    public object lastModified {get; set;}
    
    [DataMember]
    public object spatialReference {get; set;}
    
    [DataMember]
    public object accessInformation {get; set;}
  
    [DataMember]
    public object licenseInfo {get; set;}
    
    [DataMember]
    public object culture {get; set;}
    
    [DataMember]
    public string url {get; set;}
    
    [DataMember]
    public string access {get; set;}
    
    [DataMember]
    public long size {get; set;}
    
    [DataMember]
    public int numComments {get; set;}
    
    [DataMember]
    public int numRatings {get; set;}
    
    [DataMember]
    public double avgRating {get; set;}
    
    [DataMember]
    public int numViews {get; set;}
  }
}
