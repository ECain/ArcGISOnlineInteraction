using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace AGORestCallTestFS
{
  static class RequestAndResponseHandler
  {
    public static ServiceCatalog GetServiceCatalog(string url, string token)
    {
      string json = Request(url, token);
      //Get the object based on the described DataContract
      return GetObjectFromJSON(json, DataContractsEnum.ServiceCatalog) as ServiceCatalog;
    }

    public static UserOrganizationContent UserOrgContent(string url, string token)
    {
      string json = Request(url, token);
      //Get the object based on the described DataContract
      return GetObjectFromJSON(json, DataContractsEnum.UserOrganizationContent) as UserOrganizationContent;
    }

    private static string Request(string url, string token)
    {
      //make the request to the server
      url +="?f=pjson&token=" + token;
      //todo
      HttpWebResponse httpResponse = HttpWebGetRequest(url, "");

      //check for errors
      if (httpResponse == null)
        return null;

      //get the JSON representation from the response
      return DeserializeResponse(httpResponse.GetResponseStream());
    }

    public static Image GetThumbnail(string url)
    {
      //todo
      HttpWebResponse httpResponse = HttpWebGetRequest(url, "");
      Image image = null;
      if(httpResponse != null)
        image = Image.FromStream(httpResponse.GetResponseStream());

      return image;
    }

    public static FeatureLayerAttributes GetFeatureServiceAttributes(string baseURL, string token)
    {
      //get attributes for the layer
      HttpWebResponse httpResponse = HttpWebGetRequest(baseURL + "?f=pjson&token=" + token, "");

      //get the JSON representation from the response
      string json = DeserializeResponse(httpResponse.GetResponseStream());

      //Get the object based on the described DataContract
      //TODO: can I replace FeatureLayerAttributes with DefinitionLayer instead
      return GetObjectFromJSON(json, DataContractsEnum.FeatureLayerAttributes) as FeatureLayerAttributes;
    }

    public static Self SelfWebRequest(string url, string token)
    {
      HttpWebResponse httpResponse = null;
      try
      {
        httpResponse = HttpWebGetRequest(url + "?f=pjson&token=" + token, "");
      }
      catch { return null; }

      //get the JSON representation from the response
      string json = DeserializeResponse(httpResponse.GetResponseStream());
      JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
      return javaScriptSerializer.Deserialize<Self>(json);
    }

    public static HttpWebResponse HttpWebGetRequest(string url, string referer)
    {
      HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
      httpWebRequest.Method = "GET";
      if(referer != string.Empty)
        httpWebRequest.Referer = referer;
      try
      {
        return (HttpWebResponse)httpWebRequest.GetResponse();
      }
      catch { return null; }
    }

    private static string GetJSONResponseString(string url, string jsonTransmission)
    {
      //create a request using the url that can recieve a POST
      HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

      //stipulate that this request is a POST
      httpWebRequest.Method = "POST";

      //convert the data to send into a byte array.
      byte[] bytesToSend = Encoding.UTF8.GetBytes(jsonTransmission);

      //we need to declare the content length next
      httpWebRequest.ContentLength = bytesToSend.Length;

      //set the content type property 
      httpWebRequest.ContentType = "application/x-www-form-urlencoded";

      //get the request stream
      Stream dataStream = httpWebRequest.GetRequestStream();

      //write the data to the request stream
      dataStream.Write(bytesToSend, 0, bytesToSend.Length);

      //close it as we have no further use of it.
      dataStream.Close();

      //make the request to the server
      HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

      //return the JSON representation from the response
      return DeserializeResponse(httpResponse.GetResponseStream());
    }

    public static FeatureEditsResponse FeatureEditRequest(string baseURL, string jsonEdits)
    {
      ////get the JSON representation from the response
      string json = GetJSONResponseString(baseURL, jsonEdits);

      //Get the object based on the described DataContract
      return GetObjectFromJSON(json, DataContractsEnum.FeatureEditsResponse) as FeatureEditsResponse;
    }

    public static FeatureQueryResponse FeatureQueryRequest(string queryString)
    {
      //make the request to the server
      HttpWebResponse httpResponse = HttpWebGetRequest(queryString, "");

      //check for errors
      if (httpResponse == null)
        return null;

      //get the JSON representation from the response
      string json = DeserializeResponse(httpResponse.GetResponseStream());

      //Get the object based on the described DataContract
      return GetObjectFromJSON(json, DataContractsEnum.FeatureQueryResponse) as FeatureQueryResponse;
    }

    public static string DeserializeResponse(System.IO.Stream stream)
    {
      string JSON = string.Empty;

      using (StreamReader reader = new StreamReader(stream))
        JSON = reader.ReadToEnd();

      if (JSON.Contains("Token Required"))
        System.Windows.Forms.MessageBox.Show("Token Required");

      return JSON;
    }

    private static object GetObjectFromJSON(string json, DataContractsEnum contract)
    {
      JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
      if (contract == DataContractsEnum.ServiceCatalog)
      {
        ServiceCatalog serviceCatalogDataContract = javaScriptSerializer.Deserialize<ServiceCatalog>(json);
        return serviceCatalogDataContract;
      }
      else if (contract == DataContractsEnum.FeatureLayerAttributes)
      {
        FeatureLayerAttributes featLyrAttrDataContract = javaScriptSerializer.Deserialize<FeatureLayerAttributes>(json);
        return featLyrAttrDataContract;
      }
      else if (contract == DataContractsEnum.FeatureEditsResponse)
      {
        FeatureEditsResponse addFeatDataContract = javaScriptSerializer.Deserialize<FeatureEditsResponse>(json);
        return addFeatDataContract;
      }
      else if (contract == DataContractsEnum.FeatureServiceNameAvailability)
      {
        FeatureServiceNameAvailability isAvailable = javaScriptSerializer.Deserialize<FeatureServiceNameAvailability>(json);
        return isAvailable;
      }
      else if (contract == DataContractsEnum.UserOrganizationContent)
      {
        UserOrganizationContent userContent = javaScriptSerializer.Deserialize<UserOrganizationContent>(json);
        return userContent;
      }
      else
      {
        FeatureQueryResponse featureQueryDataContract = javaScriptSerializer.Deserialize<FeatureQueryResponse>(json);
        return featureQueryDataContract;
      }
    }

      public static string HttpWebRequestHelper(string url, string transmissionContent, string referer)
      {
        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

        //stipulate that this request is a POST
        httpWebRequest.Method = "POST";
        httpWebRequest.Referer = referer; // +"home/content.html";

        //convert the data to send into a byte array.
        byte[] bytesToSend = Encoding.UTF8.GetBytes(transmissionContent);

        //we need to declare the content length next
        httpWebRequest.ContentLength = bytesToSend.Length;

        //set the content type property 
        httpWebRequest.ContentType = "application/x-www-form-urlencoded";

        //get the request stream
        Stream dataStream = httpWebRequest.GetRequestStream();

        //write the data to the request stream
        dataStream.Write(bytesToSend, 0, bytesToSend.Length);

        //close it as we have no further use of it.
        dataStream.Close();

        //make the request to the server
        HttpWebResponse httpResponse = null;
        try
        {
          httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }
        catch { return null; }

        string JSON = string.Empty;

        using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
          JSON = reader.ReadToEnd();

        return JSON;
      }

    public static Authentication AuthorizeAgainstArcGISOnline(string username, string password, string referer)
    {
      //Help: www.arcgis.com/apidocs/rest/generatetoken.html

      string url = "https://www.arcgis.com/sharing/generatetoken?f=json";
      string jsonTransmission = "username=" + username + "&password=" + password + "&expiration=120&referer=" + referer + "&f=pjson";
      //create a request using the url that can recieve a POST
      string JSON = string.Empty;
      try
      {
        JSON = HttpWebRequestHelper(url, jsonTransmission, referer);
      }
      catch { return null; }

      JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer();
      Authentication authenticationDataContract = jScriptSerializer.Deserialize<Authentication>(JSON) as Authentication;

      return authenticationDataContract;
    }

    public static object GetDataContractInfo(string url, DataContractsEnum dataContractType)
    {
      HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
      httpWebRequest.Method = "GET";

      HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
      string JSON = string.Empty;

      using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
        JSON = reader.ReadToEnd();

      System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

      if (dataContractType == DataContractsEnum.FeatureServiceInfo)
      {
        return javaScriptSerializer.Deserialize<FeatureServiceInfo>(JSON);
      }
      else if (dataContractType == DataContractsEnum.Administration)
      {
        return javaScriptSerializer.Deserialize<Administration>(JSON) as Administration;
      }
      else
      {
        throw new NotImplementedException();
      }
    }

    public static bool IsFeatureServiceNameAvailable(string featureServiceName, string serviceURL, string token)
    {
      if (!serviceURL.EndsWith("/"))
        serviceURL += "/";

      string url = serviceURL + "isServiceNameAvailable?name=" + featureServiceName + "&type=Feature_Service&f=json&token=" + token;
      HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
      httpWebRequest.Method = "GET";
      HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
      //get the JSON representation from the response
      string json = DeserializeResponse(response.GetResponseStream());
      FeatureServiceNameAvailability availability = GetObjectFromJSON(json, DataContractsEnum.FeatureServiceNameAvailability) as FeatureServiceNameAvailability;
      return availability.available;
    }

    //Typically this call would continue on from the isDesiredFeaturServiceNameAvailable, passing back an object containing a boolean
    //indicating success, string message to indicate a reason for failure, or if completed successful
    public static FeatureServiceCreationResponse CreateNewFeatureService(string featureServiceName, string serviceURL, string token, string referer)
    {
      if (!serviceURL.EndsWith("/"))
        serviceURL += "?f=json&token=" + token;//

      //request string
      CreateFeatureServiceObjectParameters createFeatServObjParams = new CreateFeatureServiceObjectParameters();
      createFeatServObjParams.currentVersion = 10.11;
      createFeatServObjParams.serviceDescription = "";
      createFeatServObjParams.hasVersionedData = false;
      createFeatServObjParams.supportsDisconnectedEditing = false;
      createFeatServObjParams.hasStaticData = false;
      createFeatServObjParams.maxRecordCount = 2000;
      createFeatServObjParams.supportedQueryFormats = "JSON";
      createFeatServObjParams.capabilities = "Query,Editing,Create,Update,Delete";
      createFeatServObjParams.description = "Edan Testing";
      createFeatServObjParams.copyrightText = "";
      createFeatServObjParams.allowGeometryUpdates = true;
      createFeatServObjParams.units = "esriMeters"; //TODO: create an enum representation of values
      createFeatServObjParams.syncEnabled = false;

      createFeatServObjParams.editorTrackingInfo = new EditorTrackingInfo();
      createFeatServObjParams.editorTrackingInfo.enableEditorTracking = false;
      createFeatServObjParams.editorTrackingInfo.enableOwnershipAccessControl = false;
      createFeatServObjParams.editorTrackingInfo.allowOthersToUpdate = true;
      createFeatServObjParams.editorTrackingInfo.allowOthersToDelete = true;

      createFeatServObjParams.xssPreventionInfo = new XssPreventionInfo();
      createFeatServObjParams.xssPreventionInfo.xssPreventionEnabled = true;
      createFeatServObjParams.xssPreventionInfo.xssPreventionRule = "InputOnly"; //TODO: create an enum representation of values            
      createFeatServObjParams.xssPreventionInfo.xssInputRule = "rejectInvalid";  //TODO: create an enum representation of values            
      createFeatServObjParams.tables = new Table[] { }; //TODO: test if this can be omitted as this is simply creating an empty collection           
      createFeatServObjParams.name = featureServiceName;

      //Serialize the object to a string representation and add the other params needed to complete the call
      JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer();
      string featureServiceInfoString = "createParameters=" + jScriptSerializer.Serialize(createFeatServObjParams) + "&targetType=featureService&f=pjson&token=" + token;
      Console.WriteLine(featureServiceInfoString);
      //ensure this is lower case or we will get an error back from the server
      featureServiceInfoString = featureServiceInfoString.ToLower();

      string JSON = HttpWebRequestHelper(serviceURL, featureServiceInfoString, referer);

      //code 498 invalid token. Write a catch here based on the code returned and if a 
      //498 re-request for a token quietly and then re-execute this function

      FeatureServiceCreationResponse featureServiceCreationResponse = jScriptSerializer.Deserialize<FeatureServiceCreationResponse>(JSON) as FeatureServiceCreationResponse;
      return featureServiceCreationResponse;
    }

    /// <summary>
    /// The resulting serialized string is case sensitive. Be wary of changes to the AddDefinition sub class names and structure.
    /// Changes can result in a failure on the server end.
    /// </summary>
    /// <param name="url">Feature service url to push definition into</param>
    /// <param name="definition">Feature service and field definitions</param>
    /// <param name="token">Authenticated user token</param>
    /// <returns>true if we are successful in pushing the attribute definition</returns>
    public static bool AddToFeatureServiceDefinition(string url, AddDefinition definition, string token, string referer)
    {
      //url = "http://services1.arcgis.com/q7zPNeKmTWeh7Aor/arcgis/admin/services/testtest345.FeatureServer/AddToDefinition" + "&f=pjson&token=" + token;
      JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer();
      string addToDefinition = "addToDefinition=" + jScriptSerializer.Serialize(definition) + "&f=pjson&token=" + token;
      string JSON = HttpWebRequestHelper(url, addToDefinition, referer);
      AttributePushSuccess success = jScriptSerializer.Deserialize<AttributePushSuccess>(JSON) as AttributePushSuccess;

      return success.success;
    }

    public static bool ShareFeatureService(string itemID, string url, bool everyone, string groups, bool account, string token, string referer)
    {
      //HELP: http://www.arcgis.com/apidocs/rest/shareitems.html

      url += token;
      url = "http://www.arcgis.com/sharing/content/users/EdanDCain/shareItems?f=json&token=" + token;
      string sharing = "items=" + itemID + "&groups=" + groups + "&everyone=" + everyone + "&account=" + account + "&f=json&token=" + token;
      string JSON = HttpWebRequestHelper(url, sharing, referer);

      return false;
    }

    public static Image DeserializeImage(string image)
    {
      if (image == null)
        throw new ArgumentException("Null Image String");
      //image = "iVBORw0KGgoAAAANSUhEUgAAAMgAAACFCAIAAACR/CB7AACAAElEQVR42rS96ZNk210txn/iwBEOHAEfwAFhG/McwQd4zx/sZ4yCLwSYAKwA8/DDYaNnpCeQNSCQQFdCulLfe3uorq6urp7nqbrm6hpynqczz/M85snMqvLaJ6+aa9436d7SUcbpk+dknjx77fVb67d/e9+fqbsXn8XW8S4sV3ctzrY525RlmbUt2rP5fr95//69MNBDXwgcwXdFjmNdz9R1yXcV1+ajQHZs6c3rzedPnwWB6rtC4CmBJ31yw1Wf3DE0qt+pDrrVr/+//0/ky+99+5uTYfPf//s/PV8EN9euX7t6tdlsvnz14snj9Uf3b1kG22ke72w9/afvfdt3JMfiIl/yHN61OWyOxerq2NQp7Ih8V+A62NGUEfZtk1GkAe7H0CaWQeMSHFTlITZcIrC9wJdCssmhJ8t831DGoSvq8khTxvgW/DTPEbD5jqiIVBj502k+TZNuq3K481qRuLOz8/OLC/z/bLHI5kU+zR4+fOj7/he/+MXz84Xve3EcOo5lWUa73UrTdLFYuK6bJEn84z885dlstrGxYRhGURSO44RhGEURDp6Xf4vF3LTkIHQWZwU+LU1jvIWDWZbhdVrE3d7x+tr7ojCYL6azeTI98xdnM9+3KpX92Tw1DD5Nw3yWOK6ydfSoNtyr9Heqg92J0lBtRnYmY63CmA1srNX8mc8IWC3vQnek0FXDQLYMTddEVR4kgXp4sPXq1askNnxH9iw8aP705Mi2bTye0WCIk4EVzxHDQL17797uzk7gyYEn/gtUvQPWcvNcwTYEzxZ1dRIFgiQMTY2WJCoOFZ6lW63WlStXdFMxNEaTJ2HAK+JYVSayPA5cKfI13xYMfQK4SEIPMMKGfYCGY1r0pA4k2SYLSAFYqjwKPAH7OAfHgSfsAI5ANrYo+Pj2ACOJ67kGg01g2sDWO2DhKtfhfV+xbfXifD7NEt8zszxdnC3OzoCoxdl8VpzNa6okquqP3v/hNMv/6q/+6uLiYjqd1uv1N282OY6p1WovX76s12pA2Wg0wg/Eq2ma4/F4f3//vffeU1UVBwVBmEwmuDbP87PyD0hyPRUAKmZJUeTz+RzfSKB8Rr42jJwxVW/UdyV5cn4x91PVn8rZWWIYommKXmhpBrc4y/MiTcLAdrS8SGxXU3SuSe3ySl+xKdquLVH1WQDrvOFc1JwLyvLxrbEnoZPR9BjtJ/HdKJQePXqwtbVJGsDi8Y6hc612A51V04xut5PEtmMLHnDjS7oqfHTlmqpyABZaizCBL3+Srv7TfZyAJgQvgip8hwtcRVWlzc03L58/w1tuSRiuLXzMHCVAsZmgKGUSOKKpjz2bw4XAim/zPN0UuY5tgL1oWewITMPSJ0CGLHZdm+APvAW0acoQxKaqdIjbKD9QkYayOACSTJ0W+R5el8Ba3i2+GtcGpqxpnOt6QNQ5oSpw1TkaOJtOW532s+fPVldXl7D4whe+cFH+bW5ufu1rX8fB7373u0dHR9/45jcPDw46rU6z0Xz69Cnoqt/vf/TRRw8ePMA54K2trS2KovCZuBYAAjkBwUDV+Xkxm+fTaQGiAuGB1dCrp9O0mKeKyhwePAoDJ525dsqmMzBWji6qmzxglKbBdJYut8X5NJtGQWhLMlXpb03kxlirjfTjElht1mp8+sCqOhcN9wKMFLpS6IiuY2ZZ5BiswLUQ5u7evXN0tGcbjKlRnsN5ruy65sHB29u371xcnEWBBroCktBgcexUKpWXL54loYXgguYBc2BDC73jrSU+3gGLRF6TIUd8dZp5aeLv7W1fuvQBz4LJFFz7ju2WKMRHocnxyQjKuBDBOgB8Q80yJduSLIOXxL7n8mAmQxkpQtdUhgCZrg5LlAgAFqIkgAUO03VmeRsE2QQ63JK6BK5rGcwn73b56jhi5EqaJKDVozgC5TQaDUDh+vXr9x88APegyZeYAIzu37+PmAj0vHjxQtM0YIimKYqm3uzs6rre73Vxoed5oij2er27d+8iUA4Gg52dHewQPGUZLl9+miQJh4eHGxu3VldvXL167dq1lbUbN7FfrZ7irGdPVw8OXmSzwJ2KfqbmiyQvYj+y8ylILp8t0vnZFNtskReLbEa2PMlc3u6OlWPKqFWo3Yl2ypg11qp/+qEQdDW2Ys9T0CShr9qWFkWerdMc0zR1Dr+EF0aOycYh9JMY+WrgaaDoP/uz//3Fi+fk5NCzLMU0ZZ6jBJ69duWqInI4n6FqoIElsD7R9bkl/WAHm20AeUwYqooiHRzuPn78cH19/drKNagIgKMEnPwOhbjKMtkoUIGwsuFl05COj48++OADPOjrKzcuf3R5e2tLEBhdF4bDusj3CQRdSZH6oLSl8MKroY1lsW8YHLALxsLrO2BBXQFV2N5x5D+zbKS7GiWZ4l7ldG1t7ebNdQACgQzUEpZ/SwJbouFf/OEtIMaNEm92MV0QWYagVka6FDiD8Do+Pn7+/DnE682bN/GZkJjgMAD3yZMnIMKnTx8DnMW8ePeBaRoBYptvXuzvPwsT14oFO+HyRTg7n50hXp8lxSJJs7AEE0EVsAWVhrfmZ0U6i7LCn83js4tsInXBWLRRo42fmLFAS85F3T4DRVXwT3vWsItTj6CqzVtJHmUZ2DW3bWt/bzeKHB+P1dOODg+3tzaTxAhcopySxBn02wLHKbLSbHSfPH0eBp7nWo5t2KYaeUCevPnq1ePHj8PQdAFTtAfazIDWGXJMGxEHogqbazO2wS2pLnTlOPLwBHu9wZ/92Z8xDPP82VNTX7ar4JWoMnRK4nv10+3K8eva6Wa7sduo7hmGcvXq6tabl63m6bBfbVQPeu2GwNL7e9vXr12/cuXq9eurd+/ef7BxJ7QMP5AsfagrA1kgIBsNqtBtHoLsMmq7UmDxtstaKmOpnE3iMh+SAM1DG7gemwYqTmalSQ9tv3F7NOgGvo3whDYGPsA9S2ABZPN5AYBhH3II7wJASyiAhCYTipx/DpH0cbCDDgNLXb16FTAKw0CSZBN9VBWHQ9Bha0wNdUtQLW5xgc8q0szLp9F8kRP6mSWddh1KFM5gdp64qRhMzcV5kU3j+XnmpkqUGQbMTS5kMxfH52d5+Uq22VlenGXzs/l0kY75JmcOJXNACT9FKGyUMKp5Fy13ocRzQVNYzRjbIc3ysiSIAsuyk2ajTnpfYIMVNEV49fJ1GHlERdlCHEB025LI26ZoGJJpguMV6ETf1yyLvDq2bJkCoAbd0Ou3ZYnC6eAkooQcgdCDJy2FF/4Zeiq4BNCBBu10WkEY+n6AXoiYUqse4S1CEq6C73UsHs5RZLtwbVBjePVtkRr1wU+9Qdv3VF2lIIwUscfQJ7Xac5ZtxpFhGQJF9YC556+ef7S62mt3bJOGtGIp4HvMsx3XIhQYEashuSTSIcjirpQQrhYU68k2NlehFcZ3lArbdixRUITb9x9svnoReloSu1BBZ+dniFwIW3hmS2aazaYlwBZLAsO7SwwBYVDuDEPjueFfy4NA1Z07d8BbODnP4ij2gAAv1uZQbrMoTEw/VtN5AC2lW6JuSGkWgITixA9Cq9+vbG29evHiZZr56czL5gEAB12Fa9NZCHhleehmWjq1SjBlYKxZCcoF2OsCO0UxTxipywFVYkuyJj9VKASqoKgEN338+NFiNgXlIgjhN6YRQZLnyehRv/d7v8+yzMU5Id4//MM/+qM/+kN0vKIILs6T46Nj2CKEwmrlGLH/4OBwb3dPU1WQ3Ivnz+7evYcYsXJt9d69B9Vq9c3m5s7OG5ruT0b10eB01D8dDSrUuE5so6uA23ziJbUwdL797W+vrKy022200O3btxSFx/F5EaVJMM2DPAGelXZjHwgLfS2NvXarfu3qymgw8IgqgvibaNJQEQat1kua2dWNoSp1NBlxcKyro9hXVJ66uro6HHQEfgQrGpJvF8tYzLieiCORKUaWGNu8ZXGMPHFcdSgOGuKgI41ZuE4CcdVzdGibk6PDwFHgE+FUgBzoeEgreOR3wFositmsKBFWJEkEDAFegBSMHkkQTKdBEHAcB2kFzY7HhX+WJ88Iz5X/mxZpHAfQ4DPCK9P5+TQvIs1gw8guZik4CUwJMD19tsYLY4ZhHz58gK8uQEBATNlqQNJ0ns0X0+I8K+ZEbAFD0wJbvFjAbyIg5pDzUGO6w1d6r05aryvD7Z8cWFWXoKrnzjXHvXTpR/P5DIGPZZj/+y//w/bOK98hXILb+vmf/3nP87/61a/gcfzyr/zKL/7iL+IgWPfLf/3X/VEPOmB15frGzVueZ+d5ilgTRfGz508ODnY4fggDjOfp2GZpdh5Bin10+SNVAeXokPNJpE1GNZKbIMpJThP4BB2PFQoJZvsb3/gGxMSTp49czxR45sv/8ctPHj2qNSp//dd/I8lC4Ju6Ltdr1auXr6yv3xIlJgxUOAywDkKHJo1kodPr7Gt6T1bahja0LcayQcWTrVH1jVhvN6o319ZlmQp9aXlV6CuWSdsuZ9qC6ki8q4i2jOg/kodRZPqIfYEa+2pmyyn42FbWb6ztH+wmoRmHchwqsshfnF/M5tM4jg3DAG6WoRA0BsLHEwOqQPaAF7hqmcQCKaPfgpIvXboEKbm3t4fTlimrZbiE1y4KIM8nx4GePFycz6bzxDBhmFTD4k3cSWQW88z1tI3bH+zubj19+gwf+Ku/+t/hkjDwszS6evVKaS/CIIJDnAeBB/+YpH4ce7CEYDtoF0AczFoUcJp5PvXSqeel6k8IrBp5XcD9iZaLEIaGxw+JI+f09MRx3V/4hV+YTwPPUXFP/+pf/fe/93t/gB0Q9a/8yn/9S7/0S/jxUJTdXh/i+vrKmsBzkW/4RD9Zjx89gjYC6xrm0DRGOBgH1utXL/72b785Gk1m84Klqesrq2/f7uepjUAGdzkZNos86HWblcrp28O3f/Inn4ew2NnZ3Vi/9SFg+NGVDz+89Hpz0zTtBw8e+oFbrddVRb15c+3Oxu3xaKjIkPCApgJ0IrzKfK9MO7FQ6LZFaWq/093mum8NZXJC1TvaxCKqnzYs7tIHlwxDRkQjsA6M58+eff/7/6SbgmCqZuCjqUxXy1IvTz1J4MDLUailied7SpQ4nufc3ridpg5iJdH1rpSlATCUpglgAQZ6Bw5NlxQFZHYOcgKqloy1zDwVRQGp/v7778MMLjU+MIczAUTXtXE2zsoziN0Eb8WZD0YB99iuXsziPPdPTt9MqJZpyWAmWaHG42atUoecxcm/8Ru/AV3y7NmzRqP+u7/7u1DAr169gvbf3HojCAKOQ/IBtMQVFtm8mJGmj8M8z9IsSTJQ5tyMpJ8cWA3vrGafAbQUNf7bb/4d7gm97dXLl81G8y//8j/o+liW+UcPn+8d7DLs5OWLV7Zl/y+/9Vv/9t/+W7D9s6fP9nYOhsO+Y7sICqUpAw8Z/V5nY+M2qNtzlzxEEphoNpoeD4dDCIhWs05R/Rs3VhAZB/3+4cH+06dPrnx0+e7t2416Q9cM9OY7dzbQkzRVunrliuuaYQDhb+JyBItKtfrVr349TRLfNacZ2lde+kTiEMukADOu2waN2LQkwmH/mKaqW/0DVadcS3AcKEnGswEadTIe37l7W1EFVqQ5kd/bP0TkHQxH4N3j06NavT4eT2q1ervd+Zu/+YoPglWE3b09XVN1TUO/MnT0JbiTUvz5ShJ76PHAFpABcbpU8eixuMU5bFmJm9Ijns9JcCQSR9c19Gee58u3oHGmS6UVx1GShET0FCnDjEgOfTGNEwcEk6ah6xn5NJYVdjhq5VOgIU5S7+3x6ySJv/zlv/n+978/GAx/8zf/NcBdrzckSULAwf3UajUw2ec+9zu7u/tPnjyVFeXG2s3Xb96wLIfWRLDKshTAmk6zvAg9z5TM0U+hsZw5GIuSFZHnpkVWTGN0uzCQo0hLU4vnK74vZjDFkYogEoQK6BdKEyIgTf0pOlAeRaGlSCIUj+eQtBACnGVqfhAMRwN08TDUg0CDcIYCGI+HMORQcnt7u1ni65qEndevN4/evh2PB5rGgwzevt27d3fDdwyQBCSXY0q3bq1DCQEusjQejzqBZ1im3mjWoMPKhD7B0zKtv0wQYEeT+pZGEVvgkTQE7OdkeFprHXoOY7rMhGuPhiMVz1UWTE3f3tpaub564+b6cDQBgHZ3dnBLQPZoOPjue99dWbkWhP6//je/effePTT5H/3xH5+cnlSqte9+93snJyetRjOKjNJwiBCjiswBkUAIzlwyFsnCny3AYdBa7/xgeTyfgdrSBM4XEX8xX4CZktx1AxmRbhlPEapcX+t0TgaD6uyM5KIg1TWNkcUxeimQFCVemkeOq3u+GSfu2Xl+Wjm1HQdfsbFx62tf+ypgdP369Xq99pWv/I1u6KPRQBDYZrO6vf369PQANl8U6Eb99OHD+1cuX33zZguKttPpHJ8cn1aO+v0OnsdPwVjuomIuRrzg2oZra3DdJHlt9wyrp2gtSAdTpzybcwxGklsMf0AzNbjFjdvrcWzCfhsGDQIwdMVzNSgVz5aAiTD0PvzwQ0EQ6fGk2+l1O91mo1Or1vAgv/e978MEEFySNDG20HUMVRGSxMkyX5Z4eEBNXeZXEaH4wNMfP3z88sWL+SxqNQ+SSDegyjUBTQiG8BwWyhq86Lowp9IyUwokjQfHqjRY5pwMjdrfefh867YidQ17YKtDhRoEkeu4vGcz8H1BoExz2FbTtYx6rRIE4c7ulqHbqqL98J/+6fatjWJa/Jvf/B9evXiN9n5w/77rWg8ePdza3jINo9/t2bYIwyHxPcviOXaE3w4kATvgieXQHpoZccfzvM9//vPY/9rXvoYIBdbBmQf7bxEByxRUFkau5bGWRxeLGBcR7EGqE7kNoVbMz+dB7OFxeq5AT2oM0zdtGZExm0ampTguceKL8wXIbjrLcFWJ48Vw2EWPhboHUCHk8yJjuSE6QBwoeLZgd8dkIk+BUEmjCNEDfXht7cad23fu3rlz48bNlZXVn0ZjkQy7HESepZiGVnZ9bamjQ+/jsZflsC4QJoltnm1JwgQaPMsARQ4UBQkFZNgWpAIdx44kidgQxYH97e0tkFASqiQahhoIL4rIgCt46+XLV/N5YWisJIqKKmkqyMZ482bz6OjQsQ1DR6xR8GqYgJkIYW7ZsIpet9NkGZpj2SjSAeLA/zhVscxW4FtUeYTnpYlDU53YOhXbimFRB29fVpS+5U54pkpNurwgWzYniQOWaTgmVUZPKHTcgCDLNDVBtL0HTQ23qOuCrkONae12azgaKYrCcXyr1WFYqJnR6enpoN9R5MlocDzpn5oapStjEGoUwbpGiiy/yy8cHBxA3PT7/a2tLYAMge93fud38PqVr3wFan058FyvV7/0pS/i2rOz2QKh8nxOUlPQQGVGYHE+VVXaNChLnzCThiAMQWOt1qGuMicnr54+g20yQHVwdvPzosxOTcurZmT/nCQUzi6g6uZx7E+n0WJe5FkSR+jgVhjoniuHrhx5ahTIisqALM8uZlHqSDr9k4fCZplkp7zMNEGW8M88SSZ54ieHim2TAbYsg8Zm6gzPQCfdyyFubDEoYRf6KjUZ2bYhighXE00VTUMFyZ8cVyRJsB0y1guFCy2iaSIiCboFflaWuL7L2qZumqrrcIgp166tmIYSOICLDq4KfYMk9BNnZeWqafCuxWapg7sknOdLUaC67sdZ+2UeHPCShL6uToAqRejbOh05JkvXH9f3Ut90nPFkVKHooY+QYvXG45NmfU/k2nC+k3HTMCgIRM9lTZO7dvVKFCWGwRoG4xGZL/Ec49i67+M+ZURwx5LDwGJZ+vS0aiMwa1ISeTTV822IPG44aMKIQdMsU1aAF03T9+7dA2hAV4AXRVG/9mu/BqL69V//dcRBBE34we3t7X/37/4PIrDOcrDLMjNOpNgiN0w8T0YR+4450ZQBPW7YptjvVRiqWT3dunvn0snJGxi6JZLmZ7MlqpbjNmVuguDyjNRbLJZJ2jOSwlhuiNpnhqGAa30bDU1PqO7Z2bxYZNN5bMT8T5Egdc6qgJc1lTXoSJJJLxtVKr03oYFyaI9xLEZXR6WKEhmqc+XKVd8ziKHTGY/IGngxw7FlGcgScaHq2mqzWT+tVCbUuN/vQnj5DgwjIib0GIv2f3j/4SwPQ1fRFDHw9djXoMp3d98A1rErex+PNPO+DTegrd+8KYk0wjEsPVwCFL3nCVA2hk7jnHej2oh6YCx4TJFps5MGsMVzlb2dlwNpWHYAftBvqKokGyf90SbLVvC7RLYrsH2RH8ni0LMEkv03+dXV65ZpiuJAVSYSPzIUZj5L8TsQNZaD4iHCriPQ9AixjGimcujZ83xJGKlCf3frievokwmFg1DEgBfIvF4nSeZ2pw0YIRR+8YtfxIX/8A//ANqbTovj42MotqtXrxIdVqY0Z2XKKisiy5YNncFzKOMGLcujIDCnRewHliDRe3uPWW4ANM4viuksJQn0RVTMExBVOcyclemrZH4+XabEiLwDmMoYTQoxIPpK9gJ/IVBoOp9k/uxsZuOxh44ZyT/zE6dG6w5JN3SV0LJUjgU+NBs9XqeXTbscM14q4qXhwhHXER88erC1vb3Mkv84aa6RbHjsCgITBooujc/maRDYN27c8FwvCF0chAiLQjg4zrbU69dXJF70fINmJkFoabr8IXH+6Po6On0S+4gmhK3DeJbPNzdfd7ptnhegYrr9AW6VyD6dWwJ9yVXvFLQfqCLbZtm6PG49a+yKjpQ6WmCJ3Kgu8iychGVQDP/WMnuIXPXaG4/UQeCDO4h+lkkKH+7cucUwLEmJlXU1caD6jgKujUI9dEXH5BE4FAnkbdy4sUqAhbhVzOYFEeZw7NSkn6QBnEopdOYlNVwEgR/6fhyl169fgyvBWyAVSJ9FGbkIMs7gBxdJFgIQxSwuZmR4OMkseCbbJpLDdhhe6AnSKJ8l80UmCOizcNCDKNDb7ZO9/ecQ+9O570TitATWP480z7OPRwbPP86zk+HC8l0cgQOACAsjJwrdYpHAh4ax63oyvt0MuJ8iFDolb1k5tKso9k2oVo9FUwFAmjIGqgxSv/AxyJYjsuXgv3zr1sbx0XGe2WhRnAbSIsPVkTkZDx0TAVuKodJ8/dWr57AbvV6Xpsf7+7u7uztwgp1Os1qr3lxfhw+fjBkIGjje/f23EDEnJ0eGpiGSwhCgLW1bUiTm7eHe7u52kQUwiYYuaTrsnuhZQDmlSEPcG17fKXfQ2OnB49eVN5YpJL66TKlbJu3DP4uCY3CGPDSMniw1FakDJibVGQ6pjVFEKN3GsNuoVE43Nm77xD2QohoyLq5zsJCeq9skjwocK/C5MPlXr16GOQeexqN+mgRlrEFwOZ9MJnEcfXLIWVV5KNE7t29Xq5Vy6Hc2n0/LFMM8n6agjWmRAi6IWfNSs6sax7B9Uh2EH+XxpjbqdQ633zw0DbbTPuWF0bNntyydss2JLE80XcxmhKiyIixILUPyz3Hwx9sSYe/eKuMmwdm0SLI8jGKvwMGzTJAYN5TtUC3OsonS/SlC4ccS/rzjX8jxvF7rxIhBMHoaUbXEuivjZU0StAtpIYNx8VMRd1Th6rWVdqsO5YvoE5SFBoY6MUgyUELADj3JtZQo9gEXCC9J4oppAhtYFHEOqxzYV65c1g2Z5zkYoqvXPlRUDkJyNg18Ry0VHqhIcGweNnDQaz55/Ai0UVbzEaEGxpLYPjTBslCirJUoGcvmVKn/avvBEOEjsExbw3maodi60mzWQg8SDiEPnzkYDQ5dl3XsZYkfbnsMJgOM8L2IcWtr6yBg4pE9IiKBYFOXOI5yHMR0eAsN+6PRGD+BkNKsADfDpsxICiGBYx2NRks8ISA+f/7y9eYrz7fKrqISwWwbSRICeR999GGWpZZlFsUUVGdaehwHcJ2+5wBARNfqFAQAR7fu3blycvyqWtl6+uRG5eR1u3XAs01DpePEVjWBSKvzIgfPzbMsj0hMLGH0DkMEQLi1PCKDP6RIJozTAKQFBOfT+Ox86rjGdJaEoWna0ml3s9p/ddx6uVO9/5MDq/bjrRKc1Y0Zp4ghyNFi8ChJg1mCKo1dUs3HlZDidWXikv5NBY7I0L0PLn1kmwAWGxAhL1vaOA4h4XnX1V1bYRlK5JnQdylq0ut2YLiI5Cf2jQz00pMRejB+ymnl9Nb6Tc+GghEDsgllyhFMo+AzASaWGqytraWRg3+KEgKWQrhEI9XGhknyHQiyvq8AVTRdq2w/2RlU0TT9Qc/U5YBAQdMNCY7V1GCwYUE4gW/ZkFOIpA4nC11IeJFvQ3KxVAvvVqsnd+7cDTwoRRae17MFjeTxCaGJMqup6njS931zOOxBjZFcKIN7UGFsVVWDI2bZEUUNYZNnxfQv//L/gsyCkDIM4+3B4R/94R8zLH379oZlWY1G/Tvv/ePdu3evXLkCjXX79u1ut/cXf/F/PnnyeDgcWCYni11TJ3Ufw1ETneT8osBr4GsC19eUnm2xAkeTkhg4uMQttVGe5oFtK/NF6oekzmJ+Du+QA0NJHqZ5CFSVmJsW83TJXvjnGUTYNDFtWdVoxFzdFbe7OyNtohiUalGfRj2Wc15xLvQwPz6uZmmwbCeX4IkUuePngZPRlmhmGJMlT6CjN+r11RurWeaVxTCEzyLfhF2KoqDZqKSx69tSDq0VePsHB0+fPoUKLktJpbKKS394/0HltALHNB71SD6CyHaxrA4tlVNAyucDG8Z8+OHlj87PplnschwLN+qXFaS6PEosMbTJVbARw85B//D1zuhYcjVmOMqL0C9zEMtqT1Jc5S1LqYTIlyCnVLk/6O6P+of9zgFsPAJiEioiT0P/wXgC6KRGFLFVo5hxDS0tCYLr8r5nsRyF5yMKwuvXmwh/GSRKZL+rexFEgaYm6BjTLLi5vpYk0dHREawJbPIv//Ivg8xqtQogCJ8Difbee+8VRYGr0HNw+a/+6q/i1167do0MHJEaRhYGIiUE442o1vb2k8ODV4pM+rZhcKal+r7W61U67WPbgjS8vLHxoxcv77x8vuG6ygwxbgqKAoElceYHsYN4BySRuoYFIbOzi8XsvCjO8ukirg4Oqv39o/HO6/qrfbW/x/WYmBct5lMAFkmW2ou6dzGBRiyrkOHnEeN8ixT4Qpp4rrAsUFHLgl1gCxZMEnur11fbrQYCRxxosszKqswynKoKARm5UwDNONIRLu7cvQ3CdywVbAeH/+MMmb56fQXPEZ6dZxmGmowG/V6nfXCw8/btHkOPiyliqeU59sbt25IsdNq9dqcF741wJrNtEI/r8y58q8bKk9Yx1dij+7ZvW4bm2LpFHOXHKa6Pq/OI5yVBVpcH4/7R6eGj6skLKC2SBHYF06BBmd0OKRFmWEhJxTGZ0OU1yDgbOkyRBNb31TwLOZ5xXRUiqNFo5HkuS7yuSXmefVzBd3FOjSd56nqeAs2u6ypOe/36tSzLYCZ0Msexp9PcNPXBoGfbtiiKeIuiJ/N5vrOzBW6jKVpVlykeoH+cZr4gDB/cv9pqHYypTpIFBBbnGc4ZDCsmOrwtqir9dv9Jr3ckS0yrc6wok+2tR8+frk3olq7znqfHiQuFDuEPhsPlxCLM8yjy50VquOJpZxPyQHc4GjTpcYJNS8Zk9KkAq17WIsMnNrWQVfXxeARNEIaG66KR4KRUgWdtS7Q0yimLg0FOxDxaiiCw8MkCz0zGA00R44iUIaSpAz6D0gKNIZpAtcBpwFFfXMxI2HIJ38QhkGdUKic3b96MQC6W4TmgejsMnFkRT7PQMqThqCNwFET33bv3RqP+NINb9DR1DOXRbR9ZjqSakkxG+KUK3+/RlCyKoBNRYjxfdUDs2jL/KZaD0yop1ggg4GQXLk+maycvObrhGIh3quualoF3ze3trU6nHSeOT0aE4EjgVBRVBT2JvV5HEHiKGrdaTVFgJqNRvV4Hg4LeGHqCV+yBkMbj8clplSfHJzQ9AYAQ72iadl13mYsnJSSOIUrscnhnCcfFgrQ3/FqxyB1bJIMKBiUJnUpt1zClWm0XPMRzPcuG7TUmVLtaO6SpjmbwssoYhhjGtuPqaRKC/Eh1OJ4y7nDcPjnebNR3x8OqKI78wES4hBSLUhCY5YVGBBpLHdVlOvRb3WPeNl/tDvd5TzB9XnAo02U/BWARVDnnzbLur+GdKfFclXg8JlHk8AigIRxLRwOTJDWUcjkqZ+oiGhI65uT47fNnz+dFYps8VDxif4xmVAa+LUOWhYE8y+OT4+NKtcaxdOirJXPIoJNp6m1uvnr/hz/UVPFdOmpZJkpmwoAn0LSO5NrS8+fPt7c3yTmORDGNWOdNla8K/bY6Yi1JlgXcp+tpaUxyqpCGpP4TGhEqCv401BezqFlHiE9EgUdjJ3EEemzUK0FoOLYahcHW9jao4ubNNVVV82n+8OG9LHOjyKJoSlEEYA6nJZGZRI7jGP1eDzoSyungYF8QOEHkeIFFx8NB8k+BVFYJIo9bgj28f//u06dPyoTWotzO5vNFkvqmIQJM0+mUzLI5mxezDLY/CC1O6JoG7B7t2rSmMbCHikJDrZ+cvFE1xg+tvIg935gWEckRxF6c+FHsIuTlEEykEnVOiq7mZIQbMHUjXVQntdbu29rL2mCvQ59U+3vVwU59uNsc742409Pem+POZn20z2ndg5OntDuxXEmwKc1h1ID9tGvegTB7QUuaZRpRaHsfT15QwflRYBJFr1IuCIHhksRCs6FPX716RVU4UrjiSWlsddsdx5Yglon6Jml39fxsFscJTJBtiGUQ5MEci/l07cba+vqtna2tsKwm/eREiTLHwVga5I5weHiwsbGeRFDioiezkk378tgPpSQNd7Zfu46ehAYZNvCl0C2nGQaAl2RZKh66KPNPnjxC606n2eHh/suXLyiKXl9fT9P0O9/5DtQ2Yickzps3b168eDEcDm+ubXz44WWGpk6ODyE3bR3RljNB1ZYg8czq6urKytXrq1eur16DebyOvSvXrl65trpyY231Jl5Xrq5cuvTD1bUb11eu413w9LJO5uyMjP/hS+eLeYjeyY1iMoCzQAxNkiAIHaCNZfs004K5wW/RtTEMJi9QcWx3u6eKMjJNcUqSWAQ3QOFsDi2sh6GdF2k5aDMjsmmR+4UWFPJEq+00H1Qmz5rs9lCp8tZA8ynZGfFGT7ZpXh/RUq/a2X65s9Ho7lRbb+q11w/3n9DiQHAZQR8ZPo/tUwZWwyEl8AN3vre/m0Rwv4LvoN+bIIZpHsk8J3Ci65Dj5Tgdgpp+cnIEoWpbpATUJwkLtdfphL7mWQLJEagTx5S3t3dOK9U48nDEIZW+Rq/bf/jgQafd3ti49a5UYQm7H0+1IKoOIrrdbuIcz5bBiPTwVNWp+923juVqqux7QJXmkyStHONuXW02T4pZMhz2QT+NZuPe/ftpliGYmqYlSrIkyVA8/X7/3r37b95s5akXR+6jRw81Tbl+/RqUNZz/jdUrASlMNUmWTmOIjDMYQ5eAlfFwBBEKzxuGnqHjOUiIXOWGHyVCRJsG//rVS9OyoK6AxTQxwLiGJkgSlcREU5llUSH6Z5L483lh2TpiQkqMGyLQ2PV01xInw4amTGAUfM+0HaXXh8VZsW2Z1BOfw8elQOPWmyeTUXc2m4LwgKoyrVCmpgC7ea67wkis6h6re7TiDHSfMkLBiAQ94K2QV22KV3qiMdZsWpB7A67+pPFij2uI0A/mSDIGotYbsdVPnbHI9IqWdzaiBKDA9VSYZ5ahut1uq9UwNAVog5AnEyUgwx3OcWQI7CvXVhDRyMCICS/Gy6IEyQVYkO6uUpFvEADdvhfjcdqQ9nIUB6tra9XT40Gvtba2ioYhlebljNAy4yqV5lGAVwAD9fttuASgU5MGzepm5MpyoAkMHbkksHqkRF0GP1HURFEVaCBYsGazeWN1tVqpHB291XVllvtxZAAHYWDcunWjrGHJp9MICE4jK4rCOA6SJJRlEVGb52jiPGyW+M1y7MG2uG6nsXH7DlDIslRIhmwd2xRJRbynlDWGZIPkx1dsvnqtqlBsekwS/ZxtIJhKcHC6DnjRDqmEVqFaIbGAD8OU4fvyPCHYykJwWBy4gy6ARVGjnq6JYWA5rqwo4znJjGdeYONuTV3rtU9wwxBpYGLd5LNpMicD2FN0qjSPzEDtS1XVo3i9z2l9giGXlcyJbFGiRek+r9iMZE9kE8c52qJa8sTyhR519Grz1uudW09frb7YuvkpA6tW1it33EVQzJqtjihQpEzF0z3X1FWSYS9lluh4iuXptidYjqmkC1bSFVkscp8Ye0eGzAUE1XJyukeKIOQ4NsPQgQpOE/RndTDoX7t2Q5YZGLFbG7fx3F2Sa+DgD5bJ2DIa8qR1bYGa9G8Q8InDzsGQbQ3YIUWPfVsrx3OWPGcBDVBFYE2AY2trN8uyVqtm4dZiDy4BDAGYakrPdPg8sUlqg4Rj0hMsU+ZFSHg5i22Q2clpBejRxAlQxfNDnh4YKuIgd3i4d+/BfVGQNU2WRGg1hmIYUswhLV9AhTLZJOnhgycgQl2hCzKhNE+SJIrjdzNzgOgkiaEKAKz5bMqzE6glL3QsWzUNyfWsdu3o6GgXWoJlRgzTJ3WQj6+fnG5Oy7x5nscA36DX0HW5rEQlso3kSBez2TzDE8YJCJdx5nW4I0EbMHKHUbqc2meVPiP3KLHDaUNRH4gmddTYqtG1ttQXtLFgsZrPAXYAn+JQY6EpGINPf15h1b2oOBeUP2MYJoSKskk0TH1N5CjX10IbVkuauHnbmzXduRk4e3tbqq58/vOf397eRoiAekjj0CezesbLuYel7YfvVaGHGo3mYDgIo+jK5Wtwka3G6d27dxxX8cj0UUorJ4QtQ2E5jUeEDaxWD9ZuXLNtReAHTW7sp1nkO8t3cVqaBMNBF1QEq27ZjGnIMKGXLr0POsAJZFimHG4LHU4zZN0xIl9zTPwiJfLVyLeoySgIHNeV0siGRR2NJ2AjIv89gmx0El0dw3PAQBwdH06LlANTBrYPLrfl8hP+/1sA3LG727uw1Uns9nsNcPMyxfVuRuHSDELR16sHD26v+a49m80SPCmZh6E72X8FyTWb55rGqurEMGCeyBxGCPZlSj1NYpgQ6MtlUgrAOj8j5fOmpQJS5+ezBRRt7tXHu0OuxqkAU4tVepw6GPMtXhvyeleQxxzfeHC8SSHyBpC9QoVpGL7gBKIey7ovLLfPZO2GsgzwQpBFNU4bejxyUyuw0VGT2DEcdeImDTIbETL/ohPNK9Xqf/zSF4MgeP/77+doH0shRX8uGQta2j24+pJdlMUinYzGlqkOhsPjo7c4yDODy5cvO47uknldrK1z4LnlRHtNGmnyBGT54sXzp0+edTpt17VD17ZJTbDlErxKnqd0u22X5ETo0ijIcWjA7BfT2NJog2BUsk3Bc1XNsSdOPBx1J4OmpfOWTmi43+96nmEZPGLNxTz/4NIljueg/wKHoMqzeFODjmaT2Hz48OHb44MJNQK5QtKlqW8h6Lvix9sn1qHIUyiH8c72dhKavq9kWWDoxuJs8ePMAnwgGZyGPOLYiW2WxHMG9sq7rYqpIVh7RDadFzTT1TSuKDKADGYwz8sphBdFWcYMnT6bnRdZkcxmGZ4nRQ3zPC3LU2fYCROrOtoacw1+SVpyh1f7gj4EvHh9yJujMV2hJN6INN7uinpHtAe83VfdiUIiJqXCFbqftitslPod3hD78NJ9r6iFBGe0l7LUEBZGsp0Tj0zEqHoXDevCQtiKk2h23mk2PdtAzyEMRx43mRGPtnFMnkRDW4wDfTwa+Q6suw3NZOqkQhW65MXTp5ubm0FgkpFHnS6HMkYSP1IljpoMBJ5bv7mmGHIaWIpKQSA7rgnxRxjFhfWToX81hSkn0CoBVJcvcyxjmxo9qXkOG5LSDHZs+k1nPnJnsm3w3NgmJRsqz4J6TagiGDHI88UsvXrlan/QT1MvIAlhIXAkU2cNjUVgunFjjePgKszS6vJBADdZTq8lk7ClMo4rgW/IMu07sM9KrVqRRagC/HZET2VZqVzmF2alT1xOZZ0ZplKk8N62wI+e3Fl1Ha0gc2VSQei/eXXn9PS1IA4Ng5tM2nHszko/CHWYpfGsyJLMJbUJoZcm4azI8ZkZmRoE9zmbn+eNyR6ttCi+MZKBJGVsOKyr07o0toOBH4wsW/Y8M5Aku8M7HVavclaDNlpGwJoBb3g8hP9ntdpM0ztnNZOx/JY7b3hQXYVj6bTACHZQ9aHxZxX/nHYS3Teb9qytxVeuXgvdMjz9c8qAk7leRKZVMUSWmTKcGiKRwE+ur6xaOlnsBW9B2q/fWn/29AlErq6SaOi7Os/TuiKix4ehdfPWeuSZpkHzKiPqdOC7JKHq4clIWepnWYjP4ZimLPQMdeQ4PNqAnkwgnPGlpkqrfF81pJpzIXgxDpo65/uqKHLjUXdZrkhqfjwd/R5WURIFmwyik+M+bIEDT8c2GycPHz5AlCknaoP/yFfYllSu0FROyfcUXZOhn8IwzCPgUpqMer1eq8z1S2CR/3SWPUIax/RZaoSPS5NcVUWGHQoseprdbpzcuvmD509vrN34/pOH19+8vre99ThJvcV5HscObh5fB80ax2S6zqKcfRoX9oxMbibJCITyILZeH9970zykraATpIzn0r498rKBn4q+xpIYr2sBr3pj1qxxTp0yKqzVYKwma3b0gFMcVvO4zwpYdXNO6Z5laxMraAUgrZmkoAHZrls0jaJlzwY2urYKSzYIzxmOXVu9lmbBO3nkkIWyFFIlbJCFOgJfQB+3y8HmeqOyvr6mq6Q4B71fFgfAx/bW5pXLH0G7AEOjwThJXV2G5SaFyPfv3nuz+yZx1FN6IOtKu9WMo7Dfa7893Mclr168FrgJCIPU1agclFwSe3s7u2GgQaGLXEsTujzfMXzDIjKfyROXw/mOTapyEaZNDihptxqmYZycnu7tvokDCwoMps9x1PGoBxRv3Lrd7/UsQ4sjh5AW4UtoOiVNIbZkxxGCQAEcywldOckkOyrDjBrNShRqoFgw0CdXbfh4ziBZo2bmefbQN9sqA2V0QaLl+SyL282T3e2HI6bFs32V1N8502noh3YYOxCj+K4kMaJIS1Izn4ZnF0WUOvnCn59Pw9jTTZHjemGs7Z7eG0ntCddVvaFJGEg1QkYLKcNnLY/RPVr2KNEZ8GZHcvqM8fGaWJzVFu0+q/UVl/nMGMua87rJWQHr+E37vO5fXF9b/8GPfshlF5c/+qDaqJDq27MUtCzEBbzPixevcST09VnuQfPOpv5iFukSRarwHMV12RASVGOgi3f3dh7cv7ssd4b5MlQ68qHBvf2D/Tv37pqmSVSOK+gW32q1nj15ura2urKyun777sbN9ZuA5NoaqKVSqQ0GvfF4MB6Pdna3V65fv7ayglf8/+q1qxu37j558vRgHyIawguQoB1DGlFkjMVxLNMQPRKvyxy9o8Njup6pqvJkMlldvbGxsXFjbX3l+o2b+LqbN69evbq1/aaYRobCmwYutx3bNg3NcxyOpcC+qsLZpi6JLJnpNCvSLHMhKhmqUqnMZ6ko0Mtp9aWJO38HrDgMX+88ah/ubIn9NNGKRdbVlbQoEMpgD7OclODNzuZtU0kWU1LnfDE7v5gFvgo7FYU6yU6DtFLPDy0yZ3WR+b5xevz67fHLbvtQkulaf/t1Y7vJ9VWfNjxFdXuaI0r+BFREOx3O7rJ6nbObnNkSsW82ASzObuF1rNZYs82arc8KWBXvgtFNRrchTZrOjAmnf//33xyMJ0EYAV7/8//0P06ns/v37zSb9WqlCm2bpum3vvUtTdfRMNvbO9///vefP39x6UeX0tQqF9tAV3NVhayj1++11m6uO45croyghL7G8exw2NdV5eb6GkePPUdpCNTr7e2V1dWTk6N2q3X12rXJuM+yY0NTOY5FRIB+8slwHiIJCEMlM0tzP4os/MWJK3DU4eH+gwf3rq1cA84m1FCWxDh0SOmEo/oOmQsEQYPw1+t3KGoCW3Bycryyev3SpUs8z7uuC5MZRXBpQRwj3MzyWTyetGWFhTkISG20kMTBaDQqZ0Cc/4swh0fBsix6BXagq8ohQijwHP98R1pRFEgi5fvedBFHmTudZmeL+cXZBRc6DU00kyA7L87mUzVxJrqULIAgQxbHhoU4LgcxmU7Ci91+44Ble5anTufZmGo/fLg6HtVZvj+fJ/XR7u3DRyz8XcBw+og3xjqJfZzqM7zZVtyR7E4kdyQ4Pc5qLVdawwbq6jL7tFGf6PXPjLHcxchOZcen43nXmSph+s2//UqWTl+93Hz66Mlvf+635/NFr9ff2dnFa6PRgCv8zne+8+ABWTjgT//0Tz/88APs/Ox/9rN5FoWhCS837A1J7bbJeI565aMrDM/SIiepsi5Jpq1JMqfriqKIH37wQafbWltff/7siWlK8FbMZAAmcklKgmS60ehh4IaeRtJdDnEJZIy5rMbJYoOmKKJCypX+stgJPGs46N64sQoCq1VPnz9/euvWBoioUa9B4xu6pqnq5ps3IKrd3V2amty9c/f45FgQeDLrfJ4bupwk4ZTUoRPhTFjn4lwUWcOQEY6Xc02XSy0sEwpLkQ7+hpo8ODgAaeEVkAI6T09P8drv9wE1nNlqtjzXC313Wkx1w7h06X1QNUVRgiydXSAUkLr1tIjYSX+sME1uGLqm5SppbENahaHaH1RGg9Pu4Lg9rrJUW9eoe+s/urN5HxHfDs1slj1++2BvVDU9hdMHojYEqkpgkXyV4lF6gB0cAew4zadZgi0CL0qv0nqdsRqUXvmsgHXqXPStuNNpDRuHE6/oR4t//If3pmeLG9dX+oPB7//+78PV/P3f//3e3l6r1fzqV7+Gp/mjH/0Ij+wb3/iGpmk/+MEPsP/f/je/oqli5fSUZScwg5o+tHQJcWdl5YZucKmDHz251N591K/Qlk5RzMqVlRurNz+6fJmjxqFnLHPxg27z9sZ6mdpGCGMg1I6PjxgaOkl3bFUSGZoaAmq4H3h1MBOYjMwvWs69AfJCww9C4GZ9fb1eqxekWtLZQFhd37h8+crVy5e3t17jyN7u1jRPDw4OHz66r2lk0Qroy9GwO4MRI2XjeTHLSGaAOLrzZQEWTdPLFbBmM0QreL05Il5a/i17FzTXcp7q9vY2UPWFL3wBTwYIOzw8RED9zj9+O0vDi4vFs+fPvve9743HY0mWKvvHeRgWebRY5Pkik7hxPkvSi9nYEGaLPMr98/O03tjbqe0rlqb7FqWLs3lx//H9lcpLw7GnizMnT/TQH9pjJeSMkLN9UYt44MkIhSW8sJmRSAZ5QgHHVY8RnD5nIz52WKuN+Ehp9ZF4+lkBq+ZfdO3C861hcCbA+cs2P78IvSAtUjVO5IxUMxvxNEojP4nMMLt9+zaZXyxw6LdpmrAsDamBmGVZvG+RFYio8VAUhMixqNHg8vUbXW7EWqIDvSlLW5tbN1ZWEIzGk4GhK9euXdO1JRtJCJcs019dvW7oZRUXmZjFuY7nuRoBlqWZuuzYiixRPDuWBJphGRBkSKb2255jDPrtQb8PVc7zjCTyoIRatd7r9kSB43mhWqnoCrEaQC3IAIrt2dMXT589gTDPs0SWhDzLimm+mGdBaAWBnedkcUYcwWuWpYih5XpERRnlSOQsp9KfkwVBZ8X9+/cBLJhD8Bk6myRJn/vc5/CKLved77wHHf6//fEflktqnd9av7lybbXWbNTrtVevN+M0npHJDotpEfeH1UHvtNrCjdKzRTYc1F1bOK3ttCQGan9xNs+KOJ9Hcqjn8zyZe9FUD6dGPLf6wvHp+FmFfdVm9pvcUYt+26QO8dpmyD7ZmLdN+rBBH9THezVqrzp5s1u7/2z/Roc9OOq9FOzhZwWshjsf2LnmmlX3oh9e8DwnSEJiq2EcOK4OMMnh1M1mLjR5lhpBbELQagoeMcQNw9ApGsrSWHoiCjyi3ogZ67oUuypjim9P3t6+e4fluaOjt7c3bq+urG6+fuk4GmITTDXc3Nu3h9XKSZlbJwPbaWzeuXOHZSakKNSRLBO623cdqSxiJqU1ka8sR4HIOn0qcKYjbsoSB1mWZT7oB6o8TYOYDAta06mfZ24UurIs1munrg3NxJeLb0lp6u7ubj9//piM/tqS65iL+SIMnMODl4NBxfHUOPbKBBQpg4GK8n0fCPN9N/A9VcPlyZK9wtCHscCXLpfiAGNBjVWrVew/fvwI8t91ncdPHtcbLXoylPjxhKKePXmRJuHe9m6j2UQoJCUMZL0Gt1bbr9f2nvVOvFnm+HZtVNcNGTQvx1ZS+NksSGdeUhi9yZGTiE7GuRlrJQw20etLfg/a3IlVLzKj3A5yO5w6Ub7c7HSGy/0QxjaxcEQx6BevbnF8m+V7fa4i2uPPDFjeoucUiutUnIuqfz4Oc1n3aaqpSHwW+d/8+teWQvWHP/xhp9uFKDF0/R//8R/g0XRdF0VRI39yFNquL4+00SnbqslDfur604zmmJcvnj169PDgYHc06oSBkURoXUUVeVLb7ov0ZPTwwQOSuf44JSZpqnT7zq08tQNXhO7xPKtc4vvjIcV/XhvXE3VNoiYjeLSQZFwVyxAcW4aOBqt6iIyeSIr7XJnnWJivoggoahj4WjnpjYcJ2NkBsJ7PZ9NZQfKQeZ5IIn16vC1JNEl8lxVUy4Ue4RLKoBd7nlPgZPAFWbzvHF0LSpymJ/jS5WDOcrIopNVShwGCtklql+PQBXayIh+YerbIg1ScnednF3NSCTPP4yyEY7A9fTaf8rrM+rrhW37uz+eZlTt+JpkJYyecnfJuJjJqzct5O+GthLZT1s04M6KslNZ9Jp8F+SyDaJvDQpBJrQUguyAV8dP5YhpEbpJGZLXwwIIZ11Wep/sHp0+H3GcXCr1F38qhgjkn6lkJ78dDw40SB/bt6bMX7333u9/6u7/78pe/DI///vs/0DUd2vPP//zPgbCjo6O7d++ura1BCck8I+uiaSmhY7iC/F/9wi9++1vfurm2lua5ZeoggzgCVkjhVBDa/X4vInaPt3T5o8sfzLJoOfkCSktX+B/96IdFARel6KrhOmLkvZtfv6yjR9A0AkRKjpEEIY40MuboSIpI4YTRYLCUaK5N1qFst2A1TM8WbTIHSSdrN2BHYyxTvnXrJiTj+dnCdSwAK4mCwHdn0/ycUBFZOQ2EBADZtonANy+XlSKzh4scnrcoptBUpqkvFjOOYwCsdymG5V+73f7Sl77U7XaoUf/R3fXOsGvhQ87OkvNZXiT5RbBAY18URTl/hky5QZib4hvJ3Dcv0wEmLxXUYBwkgpsqfqZ4Ge+mrB1zdgaEMU7KO+SIaPi0YkIhC/kimC3SBVmtj5TckPHpPJiSuWLRvCwo9SK7nIpIlt2KUy/KgiQPRGtIyZ/2ctzVslJ56MxGwflAj1g3nnjTiTM1XNf0Q1pUa426pMhf+zFjQTEAWIqiGob5F3/xF4IgfP3rXxMEdjIeQjPs7e6+3nzz/MWrBw/vNzut/+Ln/suXr15DbBumube/D06rnJ5MpzngeHJ87Pt24KtkUYbAQuwbD0FmWlQuXKup/OXLV9PEsQzZ0NQoVAHHLHXz1IfMoqlBt92AhILyEkVwp0bW/CALuONGBpYu4pMdk81iExJQUeWgXJbS9TgHoVPjSFLKluLQXl9f29/bnhYpIDKb5WeLmapIUPTLtawgr0hZS0T6tefZAFlKKC1drvsIc1dyE5FcgBok5u7uztIkLmce41lBO+K1Xq//wR/8rwiIR6fHnX7fsozF+ZTUzFzMyOyaH08GnM7z2TzhuJ7jasksCnPNihkrZY2YthPaz7X8PPIyOZiq8cyxYs6KGEDKz1QnkkRl5HpKkvlx6hclM8HYwjMlaZCTuYQxfuOiLH5Ps2g5PyxO/OV8w8XFXA9E3h596sA6r7kXTXvR5wTeCWk34byEduKBi7AYP7l/54MPPnjw4MHr169csEwYvj08BDNBVQSBd+/enbW11TB0g0AnYzvAhCcHvhKHZDGjIg//85/92TLN40uSaBg6WA1WSJSk66s3nj59RqbASzxcpKIIp5XqxsZtNKGqILop0EOXr1wGbihqrKoay40EcDY1HvQ7ssjJMuKdQZbRchTfwdkSKXMlK3uzqjRxTMRz2zQVXEGKmB2FrP5Abky2dLAUC81uW+bNtRsvXrwAIwLERICfXwBSoNXFvPBcwCjMp2kYeaYOQMdh4C9LQMFVBVkBKifrRJL/QkQKDgPgAKxnz559IuFOgHX9+nUofJ7nf+7nfq7sk/8EKz2ZjPwQz9JMsvDdfMByIk2haly7fRwnXp5Hs/NZXPhRqluOmuS+P5Xni6JcPA1GcJFO4wVZQjIuFmleRIZJRlF9fGbqz8/z5dI0vEiXxc1JOV2HfEUM3ZkGn5yHWM7UKIyA73JvP/113t/VKANhqudELikECGytXq3XW/BTAkn/aJApnCiy9eqJLJJKJFhCPKO1G6vPnj7xSZKJJ0s2lLYuIHUHoATrc7/9WzDkkNv4BETMvb09G9EI7qxWO9jf9l0TPg7MOJmMwWHXrq7qZI0kAyaArDl4+TJal+dZWZaj2IwCe7mmfFnIK1oWKbyBxjJUsmhMDKB75UI3vowwqsoC5DMQD5Edkv/8BJlSS4pULR4hy3aMjdt3bt+5s7q62uvUJIkrl2wsbMtIkyiOgpyQ1gwqarm0WrnWYwZ5BZOIHgX9jtMAPgTNs7K4ANgyDO3ly5fvgAW977jGhJpUKpVarfYnf/InONhuN/EQEFXPzmdkbmA5ufQTwJoyTD8tWSeMnHQaZLMIUsE0hDACD+VAwOKMLE0L8Qf3XW8cdHsVdDlZYXRDhE1pd45ns9R2NdNW8YFkze9yZj2ZuEEKTbM0jz75jcv9GaHcLFtEn3oFKVmmuwnqMudtH1JDDh1uOvV23x72hv1HDzfmBXjZi0KH1Fe5MnTSoDdAYyNYQPZOi9mN1fV+v0+ylL7oQiF5ZA6qa0tZFqDrTqfJrIACnTcaTTBfmZ5OpknkktJKsYQgmW6TJv7lDy/LIimGCVzO0oVrV1aK3FNkDoYu8MtFlMrSeDIvTZcMg/zHByyNMZRR4NuNejPwEEZJNZitS4FDCvQcCxDnyonay2lhnML1xoPTe/duP3ryCKRC0/SVKxCNayfHJ9NyDV2o+GKanZMMwtlSfS+X+kigeOMA1IV/XpTzobEfhWTViWV81A1te3v73UgOuO3Zi4cI0+8y7+V/JCdc0lk+TUxHJesskFD4MbyKeaTqLKR6lPkWLEtI+fn/x92b/di1Z+dheo3eYkB5kJEoUKyGnTwJgoHoIf4PBD/40QEEAbGVRIhfZBhOrMFqyS2pW25JPVze25fkvZxZZHEoVpFF1jyfeZ7Pnud5ns+YtfYusm9HMgLYlwK62UelYt1TdYpnr73W963fWt8nzeGlFrFmsnHioBrWItfTXiRARFqti8mk0etfjsc1guhcXu0TRAuwWi7ZfZ2iFsusEIzI5lH+Mf6qOk0h0w1gC14iXSRfP3ivWOuWuWjwale0HYc1DHkyGgG/CwO7UatpGvBwpphiMGS48LzvG7ap7++91jWoQky5VLp/797R4R6qmOgMQKA4su/e+WJjYwN4tSjyg0EfmOPh4dFwOAD0Aii1VqsmqRN4CiAnqFA0Q+ISQbu783o7DA3XZCWR+vLLO4GvizwGFqbDXGyt8IRSFF5TBTO3lvBd0XFERRHy5WlMmYbK+B4eEdoWIG4Fftta9XLv3e6bN6/293c3N58eHBxGcTzDERRDkiSKokqlEiRIqIw8z//NwYRiy7RooMPtDWArBA6iqwjtUYQIJ66CwN/de4tTDIGnyoLKsYzAONiPWBUKtpARl7hjEqhwK4w7tqNCDEGqg3fDdrQodvFYpnFRr51GiSOo44Lueansp0Y8hzRmB6GlG4KmC5DVdFOgmJ6qMpC6RqP62dnr6aQZp94C3t9VZAZsuvQX2NlFxWUvNDV3EmUW5rx8YfpD0irSWLF7+PVv6VRNhPBACQ1XAzhi6gbOTNqC4wiSSAgC2pzkJg6CjYPILITai2ebjq3Bv8ow9Ol0Crf+8+cvvrj9RafdAKplW9b3v/99eEN/+7d/Gz42m41erwdYzbbtS/xT+vu/+N8Cse91W4162cc9fRqKqaZQ9+8/bDRqLgoxMo8ePYxDU+Apz4XAouHVC/MIiC1RZExdcgz2vVIcbdt4DCwKLPAATeUuLk4A8eztvd3aen7n7p37Dx4cnwB5gFp8eufOXY7jEXLPfMjBKaKlFSAhuOQXFxcAt589e3Z2dnZ+fg5//fDxEbDHBw+azRbUQfjWPIlaee80Xc4XQRSVq5VeuwUMP56lkEurCh/O0lXeuE+SJPehQNFRx3Ggvs8XMbD9FI/y/cLNK02DRuPo2eZNjhtGkQ110Ih42RtbMW3GAN4n0TyEbGRZcMOIokxC0YS0lC1iwxQtWwkROaURMMpl7ESi5k/tWMyWEQp6A4WdAQkXLZ9LZvCT/Q91EELKC+xCOwQ+foR2Q46uus7aQ6UGlSaG2KjMp3UlYQK8CZNBvvuA2+6GCBdMliRcrKNJkWdsS45DSxb5Sq3+5OnGs83nBEH+4b//9/1u7/L8Mm/wNIA8/vzP//xgMNjf3wM49fM//19BwB0fH1erFT+wRQmQmexZUBNpwD00Pb1770uOxSomouy2hSoxqBVGXitjSZxpSDj2mS+WufkUVBg4DM30u4OnG08ODg8vL88rVfiNGu1WC2gpRZE0/KHIWrUMkbJerTVVZOlJ4bj0lVXSBfySjUajWq1CGiuXIR2XACfBnQO1jKaZUukSyp1pKPByeEoYhbVa5fj0rDQYzNYrzXWn+UDgYrEu2ln5Htg1T8RPgAriZB+kE4i2rJDgXqJYvHV5uctxIwBYcJPwApGtgiAz/ESxI0GzyThzi2iIszBCXYZwsYSyHbqBiWOlC+AU+EUol8kiBAoJCc+KGD9To5kdZIqXWV6iqIAKAjt3EkjjNIDHV1H8199uQPcvKIhGKgv5ogHay0hFboBPHj98DP9sF+WNeMtiuu0+Qw8BUTo6p1uqh6JZfNFeigPDNNVarQG15pMbn0DdELkBzzG9zqDX6/7i3//F0XCyt7evadrf+3v/Nbyng8FoPJ6+ebNzeXnhe5ptAd6XSpenn39+G2CQhe1QFuBXGFimjmeIChRiPEykNYXPNVSZ3DYMHbzgFwgjC2ruw4ePL84PAZwpsgDPgd/Kd3lZEizTJAkcyZclFqIFLirkm/y8b/lBbL0oW19RXf8JY5xZlsADYvXdu3foyoHcZbKxsdniafi+YJ5i0OSTyOufnH5Y5IL98+UMR6lCZ74oahCA8RlAnxwJpYpGQnXDhcEcos1nSa9XGQyaEP2DXrVWOYTSuczFIBf57le+NR+jwtY8Lvbo5wVQW3iAxMOZge2uiNRj+EirIWnEVJI5tqMDAJitgBUGyxWygfmiUJrM5qv069/SaeS6WS09o0jCcYzQRYmOYjELAuvi/JQix8iqHAkKU/nq0vVERROums1x56zdviyG1iFnjAYVVaEBs5MkAcXEDyxNngJmIpD00XA5Ab9bFtBAqH2cqmLTdzDsQh0ASqUprIVtd8iR9O1bXxwc7Nm2APAcMBwEFiqO8uN+t5VFjpt32x3HdGw1ilzL5IA3uK4KfO3JkyenJ0coNGSjqKmh5RJtJifhbKpFkui8AnwO6vJ4POR5tlotN5s1SeYpatrttYvu5oeAQNtLAPLve+izWQKIEjKZKIokScE/YfvVVpwljOeu0QsThRhXqDn7Ez8El3OwsZRg43s5y3dN02Tmh4nr+DqgZkg8y1V6VTqEr0Cxw/iGZ8eRJMK/neQ5cjSs8vwwxlVEHwML/XDSovl53QDLuwk5csJUhJ2tCM95jIBU7DEANSOhzYgOIisI7aJ3lS1mKJG1SlCTLQ8v+LFfPyvM11bXSryeTPpA9YNcHPGDzVrgoW1T4KuKwu68euV5ClTJGilQRGc8BDzET8f1Qa8ED47p5gZdNLDy1ztvACbjuLAjMTQRhBBt1MbjJ7aFXa7xaIDLMwYHddBQBNRek1GTPW+GyTs7r5482TAMlNxl6GkU2thlMNCl5/LizHNEKLvtdoskh9PJEJ7A0BQxpQ8PT55tPnEcBZcpDO59xmKhjrMMYVkaTU19vAF0yzJfvXoJ0Qz89PBwHwLi6Gh/MOhAPMB/Oj4+fPp0Y3PzSUH3CkIHsQV/hXsD6OFikYWBaxjq+dmpZhlBmmC9W2Z5s3RRSIV8+JMvReMwMaSobI5gmWTGDD8FMJ4AlEflqgyYQLt16QUWJDCIqzhE7W7gp45t8Dw5GDYoqu+FFmpDvq9chZBaEVXwQ4qMBVEFv4if6hBGWjBVvaFsdxQHY8uOeYD8kOHQp2kdx0tb9xgrYs2Q9TJztsK4/AiBZa4a1krWccUKYZMAN6HyXtwMV1OuLi41jSuXr1QFB8B5wyEkUdcYCCn4Fh8FP/L1KRvP5my0fpAsS7937wHAc8OUB/2eILBBGACzR1svXRB42jX5OLABxMBfIaPkXU1AvRQkG/iumzdvEuQodJVChUaRp5B4ZBkeou9CeoNsJHu2FLpy6OuBbwFWe3j/IdzfDmpxdZy83WAZ8DSKZxhJZFRVUBXRR1kAAyIAogdoI2QsuITw27RaTUBR+bmeGIbuyelRu93Ma+W1dDtwukKhf7VckMSIngwADhIMfXRyukqzdJkCMGXYsaywM1RSSLNZlIcafBIHqGiFQu25M0CSVzTsuaO3JaoO2eWrg0G/jFGVTysDCIPwyvBPyvE0RRFRmLdSF9kK0IXMJRkaW4axq5uSZkjwSRQDH8yF/JZRBFVymcZz144FCCnJGaj+1IkFKHZh7KfL2MajId7EQyEaEptoD/xMB2T2dQeWsapby5q97k0IFPCwRdfR6/WqH5ihp0ncyLWZ0bCzvb3dqNcDl1dVnndsL3ZFZkR1W4PuYDImxyOCIlld1iLPdkNBM8gksEbj4Y8+/cyAa0tR6IaAulB3ZRF7VADFXEfpdTtZGmJ2UTC7wBNRnQYbFtyNGzc+/exGq1nXAM0xk8AVAbMTxGg+Tyyd0VTVtVVTJV1Uv2U3Hj18/PgRVDpBwKE/ihhpKg2sXpGg8KmioKAUhUAGvgOV0VSlT2/8oNdt/8kf/9EPf/A9miZfv95+9uxpkWAqpbJlGs16/fT0BBU95ovccCbWdRWoBgKm2QziWFUEPFjGQS/n/v37F5cnwOoj9A6ZWbYqiBREmO1qUeTHkCTmieMagkh7oZOrGmeQvSB54HEhrnNFy2UK/0yI72az+emnn0KIAwl9/vy5oihXV1efffajIf7pF0KVV1cXUHDPz09ZbkrSXch8i2UcxTbKxC9nucYawK8sb9OnbqoBijeRWtJRZhepLl0F0dyGvDVDfmBKFp4XWTH3tWs3rKs2trLGjGjiEj0XoMyLdHVZevf2re+rPNcSOWLr5YurS5QVbVbrgFDq3X5vNGUFWZHFeuWq26rLEtcdj0qdabuNXYSzy8t6rXZxdfno0WNBEFwXVdQePnwQR/5w0LZtXRJZSDCuC6+FucTQRTO3Y/UdpVK+fPjwPhCJjccPu52OyIuWrqjK1LYlSGBAEiFnxIHJsngBNjc3u52GbUoBZlnWwZ1VCn6UJAOMw5sEzw2NiSzxiqyoMsBBFSr7aDT8/zSr8qkYe56lUIMW+bCeZapx5LmuC9wCLmzue4PPxN0K13bz05wc7M8BrgFvANx2cnKEuqPYjg8vLs673S58cTQawKNcKcGvZFgyQM8FwLZZkGZBFKNQm21b8Op37tyBV4Ef+O1vf3tjY0OWZciRgO0kEe4KDmLUMFXgHM1m/ezs1LadVe4TFqdoV+H5Jk69ziOC7Koqa5ii4+phHMxXszAzk7mbLvwgMlA9K++Z5YAPe1qQ+Si6j/YqvvL1l0JcGDRXwMoc9HMXc3amw53R73fr1Su4nKenx6TATSmWooZI3pxwOJ7Wh9MhLzGGAah7yIut0ch0gqo164frvrmgwuWLzS2CIff39w4PD13XATh/+/btw8MDuDU9W+E4EoLJdVDyGqITiJ6JzoakpomQA6aTgefqvX736ZONMPAoamLo6LJpmhB2V0lkTybDW7duQnzEke06OAvvoBA8VbTmAVCpCgeModlomJYEGUvX5dy7gIQI23z2BDLB+xEXCJhZoQGpynyE+giLOPSj0Dk/O7RMrVKpWJZVSNPC07IspsihA8gb8gdunhZyRfO8mwAlbAGV682bN+/e7dIMCd+UM7UI+KDtmaenp5C6wsjFCINirTIQyhBMudN489Gjh1D8Dg4OCjsxtCaIIsyZaIs6g0IGKAp+DSgdNM0Ah9jd3VUUFWd4cmXl5RISoH1V3iNJwJ/nqsYCLPNDAxBYhv39DF6a5ye2I0PMoa4uIvdYUulq7RDeFnLa/bmPsQbdNhaiIiBUgmJhCqXSJTntBa466DRPL85JYiqE6UADviETdlDTZ013XTJQuq0G+Kzw13xPMFs2ftLy1+Vqw/EdUxd337x5/PjJ3t7+xcUFcPV79+9B4ieJAeYYlDHmgcFZpuwYNOC2cvnyze4u1Biep0WR//LL22GEeZ7n0TcaCMRyOWu3G3fu3O12uoGnmtjcAhQ1NrVc99YE7NM1VBIy02Q8YhiGYxgA++PxgKanWV5NXm2/vLi4LJzf4A4GNBZGPoQvQ00gG9HkqF65dLDjOqlWSsPR4MPKPB5iiixB9QVNUl1rjQRu8b4xsbhuCOEh8Qy/Iz8wyZ1zUyhPy9WcZmhAb/VGrdvtyDI/HPdIigij8PLqIogCSRZm8zRJ4uIAu/AxhNDNjQKwPQHRA9kOkhxJkq1W+9HjDZ5HZ3j4LrQ4DG2On0AW9AIDUJ1py7UaRPK2ZvBxYrHcCMiQ6+vYJl04MzyaxDZYkvrw9pJknyS/7sCCsGjkphVQ4Hy80hwga91Q0FpXp2VhpPEDQ5sOWI5y4r4RQ3qrOeuykceQsag5KzTXRGWR1Y97Y+aq60Jg1VVNCzwg5+iwBaXh7du3cRx/8sknLMt4KJnEOyYq1aLfiYJKV1FgP7h/D+rreIITmIAzbt689fnnn927d+f87MzFoqYB9fviyy8sy2g1Go7JGipjaIzET0IHUBetSSRDDDWZFDla1yFRcUgZp/0wsOBuAcAEv8nr16+vrspw8WwbroGXDxanQPSatStNFeu1kiKy+FqmDqW/iBuoboZnTXotQxcmPGUD81/OIe5Xy8WH88F5fhhXdJiWKOaeAYQqhNfx9Bc5XYQRtp5btonuxr6HGldQjuYowQBvSBA5xfAgJqq8rbrKNeKjXB0ZGLQgkrw45XnKtNQ4jft9+Hdhejs/P6HoES+MR6M2xw9dHwqu4eMihh1GjqrxvV7F8zWAfUmmqzYPEQ9fxHPrdhkPkWIbfref+whDyfgYs5wLCMCW4MYSRdrRaYHtE8D8ZJfP5zxodtIxZnVjVsOV/GXDwQiDB4Rm3V7BJ19JgSs2XJyeXIxHk+loMOj1R8MxJA8Ap71e7+HDh4CFFRXP+wDHfO/73+l2W64LleJwZ+cl8MF79+5DqWo2G3BdHzx4APnym9/8o3/7b//vO3fu1aq1ZqMJIGM2C8ulCw9F+migosBM0aRemboW66LqqUZMxgC8LJOiyb4sTqNAPz+/8H0HLuT29g4gG0BHYRgUPfH5HOCzIAuAaSD0Ak2VPNfZ39+H2wCurec5lqkAdRA4gpHoS2aYzXOMgo4j+BGo3/w6M+XixHlnCOvge0X/a1H/Naqr5+pWqevZFxdn+/vvZFmao/uXywsUvPR1D2z1433XFNsZ2B5zbEiuFMeN4KOs0lDObMvsdXvf/OYfi6IQxo7lalDp3r55fLC/OSWaQWylc4xauHquZ8AvYztqLp7rmpYoicR00qxVDvu9EoTg199uqOSjDRVzpYaQQPk01J9vbVmGoMlTmuppmjzQPTNZTAD1SGrTgcK3gjCqGotLSrnsj656oxEnkbrfUYIf/1h71eNYQ1Mb9WoPPcFanXaj024XVmbPnj07OTmt1aqmaf3Tf/rP4C2DvzYa2K8HdPLixXMoB7tvd3devxRFFoDUej3/B7/yP8C7DDkvl8xbAHJKYv/d291ms+p5QAyZne0toJzAsKq1y/29d4CYgXXiqZQlGrlbfeSrDMMCzgCQBD8k36BfF431DHUQ5pDJ4gTwb0pMBjQ1AZ4Cd8JiuY4ij2PRkUAkhs1xr8QR2EvHpnl8PSj3N6YGcMMnCyAlmGiLKhad8XyqHccZ8p47ZCQcyAGE9/bNO6iiSQzIyi82ySDUkyQtsuCHkwD4AJwXWJMgTAAPuJ6OL72YAerv9xFu5j2OUDdEHWiQKYsiAyRpOh6ZtkHSRK6nSgJUZTkOW3KxaztKq33+ZvcxzQ4B1Duu+nMfZ+B9/a41IDpnHDN68mwrdnli0uDITpuW3WQBt1dX9SEPVc1FWQpKg2Gz25D5ke9KuHNBERxDCDxXHQ5b3rpuLCH4KrwRei4jMMCY0NxLEjUNbvopANX9/QOoiUCggCeenJxAAkObSRUdMcvlMqQo+Pov/dJ/B9cVuOHLl89Ll6Vv/Mo3ioOX3/qt3yp8aQCrAcb/1re+Be/sZ599mmWzX/u1X5MkGcCvgJpZuXaVIPLIKHGAAleAgDKgZwTDsuy9e/fg5YpDm2JKHTAcxzFRFECRlSRB0fQib9y+fbPRqEPwWnCjOMZsjUxwluPo3FoivT7wgaSVW5gGsQuXPAid8Rj4THUyaXmuZhqy71vFcHA6C4teVwHGAXdCEg18V9f1YlYH/gTvFbZW16eNMy8wd17dHw5rqsb5gQ4/JFumUCIVFQf8B4MBx7GAF3VD7XTa1SqKblXKpa0XLzrt7mwZ2KkI9DBMNC/UaRbnHymKFETOtOSrq31OGH+Ezvv7xwVnCvSg32+WqnWF7wlMS1GZgT2TeWrsr6o4FJ/1jFVtUEksFFSOPGXQ60AOtwzOsWWWJurdQa0oi8aiZ0MKzHwUqFYmJBNGnqoKkgwMUn/16hUEmQ21libu3r0Dbx+kEIALEEnwBkFk9PuDf/gP/0fAGTdu/MBzzUcPH/zyL/9ysdb3+PFj+OSv//p7N258ahimLCtAmb797e/Ak3/9138dvh1iVETbSwHNhZHeqoCiIAGjDoxj+o5GU2N48hdffGHbxQZOYa27KryT4K+Amn//9/8ASLHp4vjU7/3+78HH/+1f/otv/4c/3Xy2CeH3h//u9xRF/qNv/tF4PIYE893vfhduBshhULMsWwtCQ9N5AEwQAegjb6u6IQxGjSDSRpMGL0xDYPuIxBPPt/O+1FWaxlHoL3Ib31ygZg708IO2FvwZDPrwozhu2O2WVZVmmCHLjQGna7rQH1anRH82y3Z2doAwAoQEbLqxsQFx9vbtO/j19vb2gkR3Ulbxx2Y8NWMSh+VDermG8j0DgqnrBjbqDPFjBdYxo3NUv9euNprtSed8NKgMe1c91R1YWRUQlbU4l4LxqGfrBBC9wJEgBwS+bBtsAGzfRgXvETkRwlXTXJUBvwO6d5ZTO2NF1tQlReJZlmo0KpPJGOodAIvJFDKwBKmrVgOW1DVNE65oLk58Btnru9/9j/DOXl6d+x5gaPUvvvMdqKGXl1fFmw7RkyZxBSU9jj3foWgCiuxv/MZvwLdASa3Xa9i+wtlRGUKKZSaKLKiqSBAoJeLaenE5X758CU+G4IDvBfAHL310dDSZTOCHEyT1e//u/ykkY/7P//3/GI7GnK7+41/7x/DXv/qrv5YVpXg+BGI+ecx+8gmkzBjwGWqK2rLnG3DV0bgGJwj8KHHTWeD6RrtT6/fbDCouQ0ViJIk3DAsylu+5vu9C3srbGVmRm4tUChHjOGYchyxHuJ6Wa2iRBNEDKhcmHjBB3eQsS4F39dmzTfgdPM/9/PPP4UZpNFqGZXWHVYhPAOZuLMvOxE4YN1WsmNcDQvdJ1ZtYESmqjKIqBPnR1r9KoqcKg8mgc3F2wkxqusbKbLfa7qKGkbUsq2mjUTJUIl91h7ufRX1lg/NtESAzcC4RZ/76tdIxq2tdM20ZywqgMX01smZQ4R3fmxJTGRfcpWql/Pr1m3yKHKVmozDE3XPXtiw1X0v3DBxKcYFLQ+Jh8OCEgguZqzNyAPWmk0kSu5AjAUVFoRlFxqNH954+3TzKu2WmaaC6kGtqmkgzhK7JUeCgNPt0kkQ2R4/SGAEyBBCE0TpfNoWKByipUql85zt/sfdu77v/8S8gZP7sz/8syhH0H/7BHwIQo3Tlf/71X8dYP784PDzcP9h/s7u79eqVbuiQYiF9QhJCa5racaN5ASVplvfWkSRmoeVA9lIGo4FpmbnEY5YrrUF+WkCSgwg7OTlO4hD+U3GClB9Dz/MxfHhCNJslkOb9wALoXbrak8SJZclQUmfLTFaZUvnIRn9Goxh3hhflBRbvvmUUZqoeTJClLpJkGWo2Y3q0H+turAvaSLEIK+EUfwrZK8xsWec+VmBVxZil6uNBv3p1KnE9SyVqtZNglvsMWOvT7sh3uMIbrLAXLJb7/HzqN4otuHjA8rIMsEWPk4UBpwytpOVADV01jNlY8YBzqQpPc0wYx6+2t58+fSLyJJqHu6YkiqbBKyJx+9ZtQSDd3FcHJ7RsxXMlKC5PnjysVEqBD2VFikI3d0nhCh1AqHdp4oWhm6WeJLIQMd1uk6Gm8HngG6rCTSbDdqtpGira08HdIvFxHEHlhUS1zp3g4DJEoYO1DPs6sZmspGCWLNDpb7669gz/sCJRqLQD7gekv0IUv0hy1gZ8EG4MYFuoDmzJisoBz48Tn0cnCymESode0IX/W3GqEwNBc1wdkhzL0pAmg/B64irJCqsSPHAMQwdyM+BCyNwXl++ODp6dn72OUw+eBi93efnWckUgoek8cXGvKx/CWeFEcrqIin1DN5YCeCEfbtq9Fy++HI6qusE7rkwxfSfmjQTnIGR3JNiDj6aPZaz63dIs9fb33pCTrqJO61O6l3cTqlraHffQgtAVi8Hzwi0sP6Vmcus5FtAn5PPT07PT85MJMYkCt14tc7rWUCKojAN3ARfP1mnAWt1Oc3Nz89atL7649SVLM5om5W6XEs8QX976QpaZXKUYheANjcR1eFt59fJF6aLkGrh+A7nNxZ3BfFJZp103F711BUsnFRniRlJUyjIZWRznPl5THyVDlF637XsWTq/7IUNzUC8gfeK2VhZnkQMMTQiXQ3smRquaNuuG62Q+X6PC57KoSu8n9WZz5HTXZ8kQYbkDZZItImBVmskACubFCcAp4Fz4FR3q7xQCF7fFcpuuD8wRvjFJfQgswxSK9u9wPMCDvHmYosB4EXZGArDeh3QOyU59/uLuzvaDavWsWLOp189Jqo+DCetZCj9wERfTLxBepq3OVpkdC1ZEOykHYfr23dOHD/7q4N3GZFy3LEnRmHa3ZIW8FuEhNKU0IbY+WsZy161OHaedJKBy1GRaK1FqxVk39awkhxTVKy72tRsqmvxS+X4prgF6Fmdp1GRUh0ImiXyv1y5XaooKLErgJaauhBVr2bPSiaRD3j47P33x4oWum0DPbnz6yVX5YoAWtxOann5x5z7DUkA0AbjktjOQb8ZQIwE1V6oVA8AderVZhWdO4Qxt2wLEn64QmjSBiAQ4BekKfQZdwSlkQk0uDAzg25PJiGGAchOQXG/dugXAFkBMEIWcP+s4eF/V8klaePSd2QLtvFc4f5ALyxSIBz9CmcoLWYpioelyPWe48d7es9LlG4LsMMzYMEVJpiBoJJnzfIumacBJ2NACMvh+QA+SSphATEnwZDy5m4d+YJ+dn8MNmTcmIhwQXWVQ/rDtiZsmLjxytUc1yyDyYt2Uzk53/NDCUz/syvpBZCWzEEohzrAvMj81VX/iQdrC+T6t2704Pdlh2b4okYDMFI2TVUhmshqSvNmHZzJa+6MFlr1uTAhTnQocJQpcp3leZ+SKve4Ys5ro8PzIRR8APKjO3U15dFDOl9xNg4O0QY6busaPx0PLEkIPSG/n+csXLMMbCsNY/qW+bJrrtrWqdvp37tyWVEUQaEESQ8/Z2n55+87th/cf3b5564c//CRJAshquC3tyGEEwUcHgf5s82m9Xs3jSXZsQxCmuZQoj/YFOirLGyoZYBpjLXQ/cA0VJ2Bdkw9dw7aMUb/nWqrMjyxNSpN0a+vV/v7+Yg4cPujZC1S6z1eVGnZxMLWkvUU8y5Y4pnJNzfIm6vyrn+e9qwSu2bPNzzcefr9UPvA8NUk8humbhjAeNDUDitQMUKXj2Kv1PJ8+wOECtBFcplD4ZlloWhLEUILji9pqPTs+vTBtK3fyTQCkHx1s2ZZkWahNAmzxOnGi0cnCNhVFZeeQrt6PZ31wHcdiOg+dmHNiBTJZziJHgkAMBjXD4gajuh8YskKLEmW6jGj3FYDw+bb+RxNec9YVzhxOOrrClksXNNlp0Xzh5VSRQ5YBjJVnLJwG/rFssJOHl2/zk35J4ghgf1GAE3xnZ6dwd+7t7d57cH/E8F0fnQo6rHT/zq0urYwlV1MUlIwhJgIv+p6XRJqpo2fE9qsXhg7FC2ormk9DjgFyt/nkUbl8BYDdNAB1WZrGGCpKZ2sKpas02qv6pq1LENC+r5qmrOkyz5H4AgxPkUP4OZpMqPIkS/2ry4vPPr2h6mjpNHbjtrGo5KJzLXs+dNeMv/QXqzmqYq1wIjRH98V+c2EnXtjjQDqbzSNRpPr96sbjH2xsfMrzQCsqW1sPXm7dPjvbLpfehbFN0wTHM3DJAYEVFx6iKt+KyQAUBoGhamxuB+d5sZws3DiJX+/uhDFa5QDopOme6+m5HfB1NOMxFHpnasBqXNfIJ55/wlK1mCMNMsOMmHTu1+onNDP0fFPTeUmmJWCUVBcSIZoherobCrLX10NC8caqP/1o2g3momqty90OXKqjo31VmrZ6LcHLcjPpeaPfwfFL9BrBKYPQ4TyT9kzGMWjfoGOXpSZ1SWLPjw/jwJ4SA2IydB2g08Kg275547NGb6K7888/+7Q+Ieruqm/NKvrirD8ddc8ZdkSMRwAoLi8uFJF582brzp27UWjjqILB48Iqj8dCd+/e6XbbnU7bsU3MlI406jc0hYOvdeGrzRbPMpfnp51Ok2GYWgVqugH51c69DnPdb0LXxa1Xzx89egRc4fHGE8d352nIhRn868YODpGsVrN1bvVePPKrufzKZM0SQOIsy+A/wv9YdlqtHvc6F+cnkP9ertfZ6+0HF2e7x0fPzk629t89h6rE8RzcXRCFGZAB+Ihtd1wcBWAURpbAj9vtCgB8L9KNgAB2Nl/HgFNfvNi0XcD/rGFIWe75m2WZqsr5ClqMHR9LzZeFIP4XX3Xs/ZC3MlzgCSCrCuwAe/TLxA9MbK3pnKZzEFgkNTJMiWI7UAEhsCRnyJu9jxVYbWdRNZel0cQz6L39t6pEyNxw2G9WpBAKxHmfdKRBf9CsDEYlgrsihCtSuCL4qylbZ0T4SlMyRC8AmCxy3MXlme3oVr7nA+E4nY52dnZebm01BtOeuy5py4qB3mMdLdy9qrx483o86Qoc0+12oALq6E7zFqqVZ6sQXv1ej2dp33OOj0+3d3aAn9drVWAYIQ4ck61WHYqCJrNJpEO4x7EJlVpV2QjIdW4uLDBdtBbTaQCOb97s7u6+cxyHpCkokM1aA1n5auHOlt78fQAVLvWIpRbFtmCa+ACcFQmy+OF03DI0nqaGmiYGgdJuHe+9eXxy9DrwrXrl7MXTByw5evbkdr9bBR6Xa/kluKaR9/ejGJdescyt4KvRZNp2XXWFB9JqmBp6ROjBNJzbEBmiwBwdHyiKgNbmUPmW2NxKgCxmqWmYsywNfQdoBBBL+LjCtFqEFzLNGWrLJGkWjUaNbufi8mpXtxX4OlwZ4BOCOAnRWhFYBQ+k1bAlN1H1cMxpTYL/aFKR+aHh8mrKqfxwZ2fb0jiC6FQHw7q+qDr4n47rzRpvtSycY/5bT7Jb+qpvhqZnXZavZJ6McR5VtR2pb8RNd91R06aF0xCoKW/mG9g2hNesJ7ub21ujUdtzDBF7BLptyF9++SXLQqHpUOTEhTsUY8Xf2tp6/vw5QQBc6A+HQ9f1+v2eqfHXEvP59kcxpw/0At2H0dKSQTdeib5//z6EVbHiB++xritAIAqddEDLuml2u12gCwVI933Pti24nNMp4eOmrqAK43p5j6f7xXCOZwtnR5tXZ8+79QMV6KdENiqHgKuqpdP9t8/ajQo63ihKccz3fmcVJWWimR/PvTByDYsHgqlZgmnJQWwaIWFGNARWilLBOiSk09Pjp083chtzYLIuxChUYxNCXOE4nPTCjOr7DpRUyJ1B7BTpKj8pijSDA6gnCmPbFg3gMb7m+4bnAXNSTEcNAhvS5PXSfeoKZltzx16ofzx9LDTVaenZZens2bNnpkxXGrWWty7rSzylyeVDqvmETA19nf52J/Oqs2rpqahpr3ffxqu1pImKIXSs+SVnH7b7ZdGp2RBVCxyIeK8cUdOysz6xc3wsC2OCIXiBA8QNSAuXDT09Dq1WvebaqsizJEkCmxMEYTAYoHgIQzuWjrY56DUvfPCpk4RRwRlNDQKL49jJ48cPrq7KBfSGFMLxgmYZsqkeHBz2er1SqdRqNWVZbrfbkFnziYZlPlCwHo/HtkHZuLY/HfcvBKYPLweBxZHNw92H1cvt86MXwEbRW0olVXkKMMBzdfhuiiKLTdT36S+DMJotsrw9gR2EKdEURELCsVjZtEXNmwBHm2MLalkY0+VrQunTzadx7I+GnTgJFmg4fUQQ3TiKipFDy1Jd1zQMJUlQcDCKvUG/+vb1RrV8AHDKsiTTUi7Odkql/Ubj3AuQQubTf86H43PIoE4iAiaLP8rC6rXO+yw/6Vuf1Bqli3OCHFxVzyt5wNUMiJtVw1jW9Hk9f85/YmBwVTdwfL5qznoT4vnTxwMzrRrLE9I4q1V9m7moVS44u6Qvi9WgHNit2/66pqfPT8tnl8eqzCoyr8H/8dSrnZ3lIg0Cq9FoMUxO2udzqC+1Wm00GqqaCDDLUKjCYKd4FBKmhTkPGtPrnCJRt27drNUaUEygFMEFM3Sd5PmxyuEWIDKoZDFL16trxmdZFmSaD6AKZbGwDctUr3brlXcA1BwDY5cYlLqtU4gnO1/iFdm+KgKbHtjYZrPa7U5OISE4FlEUpGkgK5zlaOk8Al7C8ROGGTx6/EmnU1I0SmCHQNDimTXPV8E+TDQUsbW/vydJPGI7KP+2Php2lxisqyBwaWocRbjjXzxzMU9Fgep0L1i2r+u8rHHY0dDZybjuuGrROOXFcbd36Qd2MZpxLUETm5aDA9MfLbBwQBl9Tbo8Do7W67V67bihX4+Y5uN7RWa69kf5W6ec85nSBXw+dNYjVn23+2bIa9VOx1YpR6HKrU5DxVD7yRKMeKshOi/2j6qV0zjQKIbmOPr5i63JeAxhBPidRXPcMUNPFUW6ceOGjDtComNDTuLQeCKXuEFvcLTDYA2VKqqhZYpPnzzCo0PXmc8BnMRBHIRBAD89A9gLMPx6B3qVHwBjkFmm3WkXwnwoUeT7PuYnunt+vMUzHQuDFUshxJAsjNCGXccxaEWYQGBJPAFVm+XYMArzkZiZ41iz+QyXB1c4sblYRa93HpNU+/Dgxc72HVUl0WSV6ikKi0Mv81xvKP9NPiwnaqp8dnJSDEbDPyD/ZSGGZlD+2s0yRtVikcQhRY5ZZsox0+GwOp0Cv9HixIOHajCqJqzWyZTonJy+eL75JUn13EQKM+d6vAdHLVA0Kwqdj9nHMhZlQFoKvJ/GVbnUqu+fjcX/VH76/9GDsFZNLWtNqd1X25bKhh6/Xy63jaRio+/m3/S1qxuLo+7kstWp1s6Hgx6UpNFoAo8osgyoWnpO8WzOsuTHjx/WanVcbkb5dYGiR26uxY1wKl+6B6ju6DxE1e7bN6/f7EqSAOlqtZqrjov6aywH/7xCjf29bfMceIDvaZrGiQL77u2bKHKA7bPMEAgptvVR/4gujhyK5rAGHFMlc75JQ2wBS5D5oabSgggPIW9PzA1Dh9eFCIGLl6SuqnGSRJ2e7nBsv9U4rVYPCbLj+SYQjmwWAP+PU5QBh2jGkJ/l2/eL2bu32+1W8/z0LJcRzCBtJynq6kLApXEIcZbEcZpEAcB5NJ+WFY3RTQFCKsst7LHRNfOnZL/ZuFQUBoCdHXNAFAx4hJQVsfDwMzOe4dr+RwssY1VAn5Ic0+zY9dRm9eCS1f6zXVshRvdHYqndvjg/1mzvVaUPJQ+PDn8SomGpzR9XvHnVHtRqp66t/ejzm81mo43NBcl1FcifCfpfsJ4n1WulF8+f0zQNGEISaeBogIGg6qEUYH6lTZ3jWWLz6ZPt7W1IHmHo53pXcz3wO5NxGISLr8zQFT0FjiWx44o9OeHJxqMotEydwpdzuKINiyhKo4pgctEHChFVcbSVF9+pyI90hWt3msWR4odaBkhLN2SWnxB0P4qti4u3JNkRxWmvX0szn2bGLDv0fBzvNB0FPkLEo41qLvsAFZBhxpA3SZI6Pj6uVCoACSBYgTBS1DAKXdyiBtp43WtburlLSpL6uWgRzqkCXej2K7YNxR2bb1BwzYTTfWLIXOoBiauFMQWkwY6kZOF+tMDKM0fLnFe0Wb3b8i324nSv5v1n8oCasei4kAVXZc5tt1tbO9tDO303lJrOuqQsfvKMctnINzLadqqqUu18G5jOj370GeASYPW6Ihk65XsWz0M2QoMk4P+f3/y80x7kflrAjAxdwXNrCyihwUJNPDk+/OEnPxyOho5tRpENTwKOBlfLdy34WAyof1hTTtJwVWylBpaD9pk8FAxZZiBYTUh+eStYYPsc1YG8BcFUsE5ZHOeIioWveCj1RjLE2FQ1IKrwMyHrFFFVDK0nWZivIKedbqlWO1ZVKlcjEtMsgAQjyyRgLPh8tgxxpnk+j8IAO/tpAkCTonqmrkLgAMk4P7968WKrWq3i7n8S4iLH8v0kMzDEwNJMMZmF+UEkbsPOZiEvDP3ABPSGhW8+DzM9WngE3eqOj92Us1B0mYlwtyJaLJOPhrG+wu9OR0x/3D8ZkLWvg2xWKPXs8qQrGuekcsa7HWdxjfTNRctZNwj6waMNXnd51731+Y9EgXAduVquMDQJrJDnWcNQDVNlWQqqhmWxqsq93X1TKl0Vsv26ppqWRJGDcun83d7bB48e7e/ve74PiASAuGOrkswUmn2/8zu/A7yy2Wy+F92bf1X8o9FoxLlIlQrgfzyWkRbQLi6C8yI3QttzCCOFsEzG9wSB7WnyBIqvadASN42jAL5LFIWvOl9+OP9ZrpemYyCEVznAUrmgox0lrmGKHDeSFAKKIPJBXPBCOdMw9KCuUUR/PGzS9BTeAUHg4uTa+Ons7AzY62g0gtwG2RhnK1Z5L3cBKCyJUbMPIN3MzPE4sASAZMWKR7HpPx63j49e6BbrpLyTin5qzBYRLnosoo8eWLUcyAMGLxmrpvtf7qSCh7uXQ2LzyRPSXxwO6WummR/PdUW9Va+bfnT//kN41/6nf/SPxpMx7mOFHhQ5Xdfy/eD6dIq7GBBPceRNJ4Net/3gwQOoMrYNvH386NHG55//6PXOVr1WoyhG4Jko9BWe5tmx6fBOYPQpXAT9xje+AVTg8ePH8PHRo0eAtB7mf27evCkIwr/+3X8t8Nyjh/d/8P0f6JpOkRPf1S0gcSzJMoQkspIEMS0apmJaoigyrqMaKpozjoZtgIGSJBfB+lVFkCLGZvNYUbEFn+ERoX2wt8VQA1mm48SNMz+BqEKxEGxBAXjCJTNDjkLPMbTBZFziSdOyoZ4HvlsY9cAfSZIIguh0OtiSSFDlIR9gXs7RXgAPiCSFpZlBlsWcMIU6+54AZoYtsvwAkhmOUSzCWa6xGySFaGX80QMLY8tYwAPgfNOY/xefFC0BVw2sjKaIo4vLc1o7I/VGDvChCHbHBJB/SF32Yg244Bu/8g3Lte/evSOK0mRCHB0d1etNqKQQXu/evev1Br/5m79JEFOWZe4/ePDs2bPz8/M7X96t1a4AFbHkSBAYgFNQO+ZpOuGpqUTTHIAxgD49uAa/+qu/Ch9fvnx56+YtADGyLN+8+Tl85Rd+4Rd4nr9z546iKJPJ5Jd+6b9Hz5L9g0a9qipyGFiBbwIJBd6gKCJkC4IYO5YJ/xxRxFU5Bg8E+UKp5iePgK7/jMaNR48/000RABAEEzBBPzCgCJu2RNF9z7cKlb105ti2nsYJQw0DW68S/bbImLIRR3GxTHEdqXm7FcLr/PxiOp0ulgsAkZXSWb9TZZkRUELDlPzQdjwNaMl40iSoTnHaky1CHAVbF1pImWkpUH+BNESJV6wb/V0EVsHsvi7ohqpu9oqXtYvjg55svxtQJT2rIRPMJqKq6Vq4WO+8eQPv16/8A9zG+f73v6+qGmSs0/OjavUKaBFcve//8HtxGP0v/+SfvNjauvX57c9/BPj+9sutLVRkcIBw0V4YxFG0zIBZBQeDNq1ro+FkvVj5nitw9Cyb/8J/8wvf+rM/Pb+8qDcbf/7t78ZJeuPT68CiaWZnZ7terf3hH3zzn//z/xW+CNVQ0wRANr6r4F61BEUQiiOi5iCwXVuyUUBVDn1bkWWga7hasV4W7YCiFs6xh5/w3PRg72m1dJTPiKKuHyRdz4VMPCXJrucpM5SXReUt1zVdzwwDg+MGYRK0um2O5wCApzNfkGlRpqPUyzXWcCE2yWLDktrNRhR4YQgZE/L6ebl8SDPDLAmAOWbLDOJm6+UXHE8ASxBlUoH7bZlZprFYwE3ePTt9m6JgKZ5bF1POf1eB9XXLJA39dX80atSbYyBIst0yc8FmOb6o1S8uTi8qFX22/t1/82/gyuy/exN4zt7eu93ddzRNQuav1RobT570e4M//uNvCjxlGkKW+oA/bt26yeUKM0Cg8IouF7pjOVEACU9kGWz9ZEAIUEDL0KU0jVzXhkQV+N5y5vHMYJk5ANrW6zDw1CjUASkBtQIml4998gD1nj7dDDzNtZFsYt/Bk21LHQx6jiXl3Q1GVxlFKWRLl0G+pRjHgSzzkFEcx9jdftKqXT66f2PYrReL165jHR9s7+w8aHcurkpvB4PqfJ36ge3Yhglg0dB0XdEMeTDqu76zRGVbnFaAT3BadZ0tUFMkhRzjOAqgQJZnzs/PWq3GeNQ/Ptm7Kp07rgl4DriCF1iCRNRqh3h6OI9c35wvY6AOtfIpxxK5squNpAXHcNIgdmxH/SkLrIZ5HVtVZz1S/bfvduzQIr15yVzV9FXRlW2pSctaVsx1mxZCmwtc1THoNPO2X7288+WXL1++unnz9uPHj6rVMlwbx5S8fJAVbvfLi9N79+8psoDtzTBK55ni2mOa9gMoK+JsBvFp5jYqSO4sUwzRYUXvdVpqLsrlOibkHud6/gcl2kydkSC/0VPf1YDunZ+djoZdnJJ15CTxMNpcy/ddwFQo+GYwSEg9A+oTYKB87Sdj2ammScBYx/36ozufsPRk68UDz4WkoqHb7yw+PNgiqQY89g+eyxIZp75uSJ5nm5pGTOF/JMMLpmUAsyt6mFHq2p5muSrkM9vTIW9phmjbUgzYaB4nSTyZjC8vLzxgOqa+vb09JMeyKVFUXze4DEfjUWMtxXNos9O+8mwrH+m6FrCE0ELtECyF85+ywKraq1wpaV221h0rbQ8He7uvRV2vWfOOtb4yVz1/3UF780XHWRN2lvjw/oiqTNH8FGrWxtONk5MTwFVx5ISeRkyGFEV4uNrF+a6M0h0XF7dvfoGbiYE/BupI0a7j4IS7JagqPx2PbOO6q5mbfuHoqe9KQMJNQxmP0I46l17OnVQcSZZF3VAcV4Yg8x2eIscQzbYpQFicnZzSFFmtlDVdcV1IMRLaEciUbZuQn6Dg4sRBGgHYz+Byp1Hl8qTVuJIlwXO0NImh0kEyi4G2+Q7LjdvtS0VhDV2Big+RMRgMOZ6FtBQlQRj5xdCz7Ro+Ok1c71LneloRBJPj6SjnN0+AT0AewqmsRS7Mt86gMm4+e6ajGI4ThFaE/Xc/79AiSyDIXpYm8BJQSeE3hJoOFFXVmTA147nz01YKPzz0ed1ZD0Wj1+0C22uqYUNPG+H61qMnnK63nDWtQnaxXrzaESxHs1Qg9u1WfTwe5dpUh0B/HIuFyw8VDcJlloXNRmU8HkShC/AF6uYXX3wRxfF8hie4/U4b5bvxwBgPp4sjaihnoS+bOAfLY6cA/2oM+h34UYFvQIz2ex1R5IMAHWJdnI8gfNd8+24PEDpE6unJ0XKRQSj0Or3f/d3f1TQFimbkW65nA4Wb0Bxcsvk8CwKv8J3zPCd3jKYhmQFxEwVK1+QwDNfL2dNHt6eTligwUIlKV+VqtQr5OC98uIO6WOJQMoDrKHJxMTUfE41nYb7YM4MQgVyVzSJIYAjAcUY+KgT+AEtZtlwqXVyWryRDArQOMCsfsIkASwEMzUeiPdtWg9BUNHoyaUItBn6qWjQvD39aAwsIZsVaT0LcO20B4bKNmrVqG2s9XW0+fmqu15/+9V9DBdnd3eZYplarDnrDhw8fPN54DIC30ahfXl522y1Z4jiWatTqDGYmi+cogedtW4H78v79B/V6XVGlMLRxWtrRk9hBQWWLU6QpHsLIU5nHMQRDoe1cUjXwNdM0r66uqtUKSRKerQDYUhXCc6Haog8PZK+Tk8M4MDxH6XbbkF3gN3EcT8zV4V+8fPFkY8MNwka76S3XkP+gMM3ns3wfEMWRZYULEN2LEHOAbPDgZbEgp/3L84NWs0YSRADf22hUKmV4rVzM+Fr9cTaPLEsNIwc1+0yB5ad+CNBe5QQCckxh5wSEDhA3gKe8cYXfC1+B5JTNkn6/d3J6ZNpmtpwtlin8HEhXLDddoi5XmM1D+CGKxqkqo2msZavTKVyQ05/WwIKcVDGWVWPpJrPHG08dW+laWU2fjdw1YGRa4Dbwrl3/1V/+Zb3V2tzclCT56OioVC69fft2OBweHx//5V/+paLIUFAockpSk26nhZJ6ukYRVLEq+ODBA8PQoR5Chjg+PDo5OmLosSyQgWd6thh4ehioAN9Fnh32B9/6kz+djMYkOXZdI45dYjq2LcVxVFnmGCiBo6FpYgk7Pt63cR1DsUzZc3X4CPHhew6EfrlcBlaxv3+0f3ikxis00cRRTzUf6ppbAKtMdb1awP9fLua2pQGkg7r54tmDdrNC0zQ8UxRl3wdah7XP8y1c3cll/jzfXOIIDeBGGwK93ys3G6eSTHs+joOu1jMIKWzWQ/y5WjIL3k+7x4UScxQDHLD2Dw+evXhGEJMo9iSZsV0ooDGkNNPRrrVM5xDBgaIy5xdvglD7aQ2sXPd7VTPXIzPrTqbVcmmqu1Ubx2yueqM//9M/Ad6UrNff/tZ/oHi+2+38q3/1f9Ubjd03bwbD4cutLYibb/7JN23LgXgajye6pnmu6rucH8ANN9F1E4j04eHB48cbAi+GYbSx8QjAEBQs37V6nU6WeoCIcc1f5CuVGjB523Z2d7dgw80AADOiSURBVHcD30QDOpODsIOEBznJNvQIzeUk12VVGcf/ffRIF2wjZwAGWhk4pgy1Dkge8MkkCi5KFcFbNKrl5y9eSpKIx8f5cERh4QT1XeBZqKdZTksbrStNk+/cuVOConVxkabXEo/z96Y3uZx/hGpYIUB4f77GTbQw9WdokQIAy9ANcY7rhwE8B9gfblpDMCUeFr7lbLbKwhhqZbhYzdfrxXA0LJWvNB2bW7LCAKEpZHDjBMC+mAst5ZEaez+1GOsrhzwTSr66PO1pyWWu3ka5c0lWpHTNZGtJ17Voxoqi4zlwsTEY4Dooiq4p8I7aKJGKS0EMM82SiJiMq5XqZDpJ08DFbUStWi09fPjwe9/7nmVZaRb5ngX54N69BxuPH2zvvIarKwqs67pAx+/evZPhKaJr65yFi6wsIHRg7z7wL0D6DrBFrlq5Gg46ns151o9X3663dg3WwYUlPPkmRE5drO/euTueTCBprdbrIm+hg2G++Aq1EbDXk6dPJJ6fB/FkMIJsl+TGwHGMrSndVApRmmL+Ll36ZsBJ5sCK6HhuhrEpisRo2stVuDEzYfMpdgMIoLnvuKoN9THzIc5m72eUg8gudOFxUnSWnZ0e23jIjdp/aJLtm5NJa0r2EjzqWSArXGQ/9YFVs9Zsuu5BWaeUpjWvavOaueq7q6a9rLrrvrPABoS97gkmTsBFNuQhlmVCyBsCG0e6qUosw7bbHUFApd0ksXRDFrhrf6/A1wlqmAOXCkNPoDBC7uh0cOn59ZtdQ5dNUyYpAoiYZShxCCWSt0yB50icnPFNQQB6KLkWYxm860pbL58B+s7NpK7h//Wkl8EUUrzwianxNM903TXD80noM5zIMczBwcFgMPj00095jt/Y2Lh96zZ8wnHcweHhkCJiFJWcz2Yom4tt92ViWHKarxOiCOAytmLRjGnVn1oxly0iRSJevbwDoZPOArS2yGW9obRBHRQVojeoOr4OqBwqIOB3CBqIqgwnYXKvL1xpjPuDLsfTALYgq+GclkqeHO94npaPkiZ+aMHzf/ozFpRFfTnR/XKl1NViwXFb1rxiripa2si1teoGzkCrs7WhcXjqrIg/+vSzWRoWej2SwIehi4nH1gobaUBI5cuLwJPz1Xsc+Hz39s3b3V3XkjyHURQqihzIQ0DKfHTEBFyvuI6KJnUaoHiBYQlNxdWx0Nf7gz4kAJyZcTSWIZ482YRYLAILqGXRtsiDiYVXyfkmD2XR1PmWuQDKzthJx152RpNqpQLx1G63TdP4/PMfQZEtlconJ2f1ZouWpRkwQ7iiuRVAusADYMBSEDeGraCNwMy3QnRS1bypGbIk3X37eqNWO8E66CiayWqGGOeuJ5CxVE0EUCWpHJQzQOhAKnq9io6qkEaMnoZ48Ay5TZZFjmOwUOar/fAEw5TiFDitdnryZjxu8Nzwpz6wiqnRvrc+v7xQZKHTbhHusm6tPgzCIxSz11MrQFsoSzQU8u6dR71+h6NpAD9QmByTD0KjVq16toB/RYEXB66ubTKOLViWxDDUnXt3DE3VZV4WaN8B3I0T8RBVTm4s7Vtyvn8hEMRYUbAl5lmQ7RRB4HAXVOaJ6TSLzO1XLwSB8XGdWigSVbGmUaSuoothWKzET1pKOPVmA3c19FZkvFaiFLek0egQpxKg4H319HC+KKqVazo6is/OYtxixV4UFjJ4+KlqRBQkLcUiSKpdvtrjJXKOshGQb9xsmeSz85HjmY5nzZexZopZbiUXxl6x/g8vDuApyXw/slQDciWNA49QFiEjIrr3VuslfFd/VHFdDcd1VtnPQmDl0bM8LjUUkTMUgVHN2lc3f8xlxcLVfoqaegZnmdKThw+r9UqWhsdH+4C1HZNxUblUjiN3iaMms263C5AFoPRw1Ol222kS7u6+qZQrgKxNQytdXpFTIktcKFuAwSHNyDJwJXvQ65nIBCWA5D4kJJ0BOgnoxwACiClNeLe7CwTRNVF+sshVxdRoEVs52ELbREOhJoaPZ+3mfJXvbOHB4VcmZ4pOdyGSm396rQO4zIVri/5ntohT3GtzNZc1PM6MWCumL2rb9+5/olmUYlPpMsil1aQ4Cz5Im6KJ4TKGUuhH9nvfQ+w+FD4oDlCTyKB0leAFHUjuKo0yHxgMfJei0hw7ns/T2TKDotzv1X9GAgtKXldytp8/CUxxADDSXv6EvU+uaDrQAYWEjO1Mhr2Np5uGoQyHfc/R804pBYCmWqtC9MiyfOPGjU67g/Z0Dm4jQoEbDLsvXm56HkB+Nk2AtLPDQRdKHtRQEcCZopLExHMUjLP8PAfqpqpwJDUJXAOitvg6KsIPu/marljMixboqkBa13tBOqeo/MRbVOw17a/yc5LVcjFbrX688loMQL+3Il+g/F8eT2HkFXaBEAqAmaA4hjPbThh0gw5oO6Gr3TeaM5Tt/oA6hbIoqwxgJsfTiqXqXKQZix0nkvARHaDyH4uL/AnKIUcI8C1ADJVm0zBUuPU6nVar267Va+R0CDU0QNCW2Y7a75V/FgKrma+RkdH65eaT0aTXnPBfHabIByIWNRuQ1pI2/JrgpGl0+8u7gsB6vheg1Tzgbt1zjYury5dbW48ePTJNczTuJ4lrW3wUaI6pMhR76/aXQeDkQ8b5Go+DrirAHLF0OiLgLUunIFEBvzMNgWMonmODwHRt1rNYxxAAWtVr1cm4n/sRC2YeUgiw8jpYeA3lgcWPWbYBv621NFHSFmCzn88OzsLQy11xcA8MjTDRyCQJfPeDx3ixGo/TDejG61qubMWsFpBmRNkpCw9GaZoR3Rwfn1dewH+yAyVOPWCCs0UmKTyao2KEAXmWUjQD83PCiHZigkB2e6UUKyxiOEjbO6/f7B0c9CfjHksxjl4u1w73D0yV7w9qhwcvTk+3fhYCC0G6se46y05vCECYnFINc9Ywv6I7b69a5rJmrEtQFp2l5nsnR3uddsP3rK2tHQAlo9GIF2iCIFiObbWbuB0VoQ+UpqJTtSgyUehubb3cP9xHizz9mjMWoWCjYSybn+pwmsrrOpBKQE5yADFnoVGUbbGWhntdrVaz328577+xWDIz8uH3YjkWexDyVDQc/EcZ6763CtD8t9iLTwvnOhx9xsBKodLN3sdTkVd+XAfnEFiWYlFGwjgJi95d6AuHA+mi3jm6fNiZHqJBnDeV7IkVienMI+kBVHPb1jhuSNH9+SI1LT6b56fO84jlxiw3BNCWotoWvhZu9yxwkgJSY5UZcZE3EvlX7/bajdLb15tXVyc/KxgL0Lo+n8gWjbJDaUX78SD8MFhHy7WZzXvOrOysKtbCctV65eLo6IiYDi8uLkzTchxnnpMpVdXgzeu02gQxIgjS9+DmpG0Hq9ug37p3/64qU7bOFUutRSjkW2KUKI4gRQFah2AF8O4io7x+DiAnzxJNjRyP+9Vq2bXRhaqogBBSHyKs2NtxTYZzgxreAGvGTVfvlymK6byvwqzZe2iVdxauveCghOEy6jINMs2KGc7o0kqLlOq1wYHqTxR31JueDIgjCCzJGbJaozHcM2PKDFjLURaLmevJLN8JEpmgu6oxtUMpioMEz23iPHCxzGVz/GvuFWA5rgafBPOQVFl7FjW5SXnYHxBjVZN+RkphxVy2zFVDsUVRLZvzVl4cixVWPlob4dxP4F1fMf5iormOzgyH3be7b6HYpam/v3fAsrymy6omN5tNYjIKAiMKgc2hj62La2G4s6Up3JdffikC/sE2JgpJFLJeGB/oQMbJEpv7kwOE4lEvHpi8QirS2NBIU6UtnYCUsPduH0ilrpFFliq+vVj/uu4+6ExbMIDGku7sel8mx+fzeSEEP1/i5uoKpQDn73U7Vj/W8IAgA9jOSX3VG0PEoFdvwlgpY6KHJaUF0874cMpedKfHkjOa8GXNG+sRPIc0Hc50+AnXMGKalJq6xWBlXCW5FTmakEEc25ZEkigvk2ahF5pJ5vmhDajO9fUkdpMs9GcxoUuKqQB5/FkB74U/ipbsAPMK14UpQUVfNM1V21u1bBSUxwYE7oqtKNMhqfHLly8DV50lXqfdGg4HrmsFoS5wnOPoWL+Qr4myJNu4yIV+AoGvf3H7cxnVddGI1Q9005RMXWIZkphOXVe/TmAWB1ECsQI/QeAGKM2QT9cUcKper3Acoymjwny6iC082BbGvi3oMjEZdvRwXjdWHlSdCFLRrADt8+uF+ghQtu0aqiF8UPTLFbCwveSGVqN+MuhX9o+eK14/L3+MmdCYkxIA75wVCpzcHVEXzd47PSJVHyUedB/ij1E9SjB6RkToEQGJLZ6589WCFaaWJYjCNJ2Fhq0IwqjdPoe7CC2c8KQoiGIHcGSaCx4luchgfoiU/R2OJn/U5nuucoYwS8+Greplo9sPFlAcq24eYdayWlj0FOIO5nqk+RxH3L17N41sx4Zcw0WBnTcnIW9JmgZxQDumEHgaQ5OokmULVm4k9vbNNsvRssyx7P/b3XctN7Jl2f3N6D/mQ0ZPinmSXjUKPYxCipmOmFBoum93X1O3imXoWfTegiQIwnsgvffeJzxIap9EFfv2jPQDZGQgQBAEQeTKvdc+Z++12HazyTEUWj4IHIhwoa+9si5A1QJeC+YEkFqMDWY/kkqPBV2jg+9ThJlEpaAIfSBhFpLZYtJR4k/RyFVbEzTbGCM/nJcsRA2zwa8RxBK0tfedYKE8+IweTEdhOgwcgHskGxGFhLKHnD9U3YHoD6VoaAwnsSgwhcKNwDOGLbqhrqjcYBT58N+qKsX2/aHoJspw6j6/zEyNLz5e1Gt3FNWBsARFIrAuhmmORmj4bIBsDcPZPLMyWNgpZqsS8DYywfrR24lYTWQOlQKhIbqtk9MrMXkCbC00SBbg+7ZM76L7oiqtra+9vEzHo1CRVccGWCgJRJ3AylbhldDTfU/hOWqEtLUQzubTwdXlBXJ3Qrp+QMwVVAwiPTfVXtR3JqfK+DcOniEGfgQgWzQiL/ZwQk+5y115jrmA3WIFCwIVHKZK6QqJ9xpp6AE5B3KM97oPQh+u/zBE+jBZcBouztyrVgJaC5gkC/sTOOYvs6eX+WDiWwnjD+VwYMyeUYsVWi+dT/d2DyC+sgwBteTR/t7J8eH+7t54hFY411ZWbF/yhlql+ri09K7RaBSLjy8v80nWdJWJgaeSRNzdHc0y4UnErkbJ0zPAPfGBn31/Syi2PkNpGb+hVOi9tMyhLmIpcq8Rl1fWO/ZoQb9qi6iWDfnAbcl/kQz1w9JSGAQ8x6DxZwwzTV1TZVWVOu025DvLUKMItaI4phZFjmHIosTd3+VliXczn9jA4yE/AotyzKygcyWINxCZAE8AplfQvKJqsdlsaSRL441aw7E5B2GL9xzW1Cg0GK0zrs5A/ahqspVGBEfUmlVaV+fZmnvm4I2MmVKk8z5YMKrRGHUThLE7GEeLViqUFtEqvBeO1MEcnjkeIhHRbysRSRL7PqTLKpQCH379sdWsmqamalIm+kBEY2X2knxc+tisV7u91s3VBXIudmSSbBmGBESK5boEWR8MoBJ0hpNwOIpIqp3L7RNUfYSWLdBOZTb9g2ZQ3g6wshJ9KnGdADVL6eVKtaF5qEH+N4ulma3wpOS+KEG8sbEBAATCBORJ4Dmg9FCih5nDhefajmOSJNZs1HC8zzGsZSphqN/d3kESdCGpAUpsAZAB3MhDSpOcLGKAkgVtWmzULPj4q3DNIvGZyLBO2d/ddZCAL6/LFGJjpuC5imaLkiVUxZ5qaYPJNEa+h8hb/DkzKp/OkMFJFLu9Xg3KMWDK7VZJ1TjAhKzx38jWN4ozZlgcyUkuDHmyzoUITTCPJVmEKrfb7RQLBVHkoUSt1+vD4RBon6xw4VAHlpa/u/3151+A3pEksquA8gVIlShRw1GQDH0k7Db0R+MEw2vF8s3h8Uq5eGbbEuorfBpBeTGfIxFK+MU3Baym99xsVaDaR2aFjtHR3Kr7dC45r/uGzYyNVdxnOpysb2wIIpvERhxpsiAM0zgT4oZcJ87nwzBQk9ixDCT6Mx4GUWAmsXVzdakqEupvcVGnsqnShkoCvCAJQrwBkrSIUsDWdZV81RiH8KZKmK2z8ATH4YHDHR0eeKaoCV3XoCmFMTypL/TbEllncdV3n16QQAwqAdEQ2JwhsTDyPN9yPVPVaUHokXRT1licbM7maRDbs6fhYBzPvzdgxZn5G9wudmmy3UMUxlDUiSzIevBTQaKjyIsi13HUeu3BsmSOJwwIlrowmsRQ7nm+DnhqtYoYVu73y46HtpkB3EnqRbENmbHRvNc0BidrD/lj+KCSQTYzPZ8+P0EiTHud2psCFnDzs5vzCC0jiYYlw1mC8upYtJr+X6wPO6hhKzU84/j05Obm5iGfu7u9LTyU8/n7VqveadceHgrtdrPdbjSb1ULhvl6vtpHZWKtUfri5ybE0fXt7c5u7usvlWIYaj2Moi4YDfz5LwsBEm31Z9QfAijJSBcDSFYIl67qMoz5mk6WITv7hPvZMqOwiS008De6kaOYY4tQEddkjtdtsy2Y2TxI0aTGfz+AP2bZ4e3fIUPWb60M4fYtR99EUJSC0ypDt92Uu4uPFxjCaifjupgml7tLSH87PVk2dOTxYWV35I0W3RRFbW/nT+uofCw/HzdYDiVdVJCtvVurXe7vv19Z/OjnZLBTOi8VL3RDKlXsvNNKBD9ErHYQ024JY6HhquXLtBRrAmhdwoKqQCI8PNtY+/fmtAev09iKJNIgomqESulMzZiXFqvho2b3mP0NIu5Ul1ZUETbzK579ubQSOE6VhAEWdKtmuDYHBcyyW503fhlIxDqDcM7LmUheqodubHK8ppmP6SUBYKq/IxVaNYiiaptrdNi8KtqUYmUy85/CeJxgaCcFMFnoS31WFnmFQjs41GlXFNTsSXpNJHmKQLg+i+JvpF9Ldmk7ns2zj+el1vh6Si2lK3U6hVr9n2IbnqrP5EIoy4Mhh7GimCLezbBELKPZC4vHpZVEzIk8KIGmdTnln+8PO13cEVVlZ/mHvamsySW5uDtdW//z5478GoVoqX+3ufBR4XJIZQcC+fv1V1ZiXl8nTM7IZT1NHQYaraTLwJZlEyuHIJTp0XQPDKpYtDgYhx2OTcZi7PPy89Ie11V/eGrAem8U0gPylWAqvGJ7sOJzIXJJkz05b4aTA8qKFel1iX6U77c9rm9dU+4HtyjYkKT30Y1miCInGOLZId3GJojgCPrvAMwmO7HL4yXVOUWXHBI4liQprSZxkCLajyiJtu4rnmaQi1smO4yiOJUW2EFp85Kk8U+WFpoWSI++6UjF/G3iolR4ubgDSq5TIYkl9Op0ijoJcuyaZNiR6GDlrmsi7CsqIWi0XhQ5F9br9kqywg2EchqbryjjRzur/xHbkTrekGmyKnE6HQPbh3W6t/ri788vG5s/VymX+5pDSmTSNBJGk6RYEqnbj4eR4rdctpKmN2qpco9N57PcfDw6WoQw8PFzZ2XlfqV1D9TAax/BXJtNBtvqgIg18Q2T5fpi4piEf7K6trr9/qNxVePqNAevlspg3NajyGrXqXb3VjyM5DZQw0VWNf6R7mq9MPTWI1CTQe0RndWUNCFkSKkmgOo7IcNwwhXQmDwYRLzOKwlIC0WO7jEiW2Y6bOGfHJ4omei7QJsHPXAvjAH5XI/p1NE4Y6DJLp4nrBWpH6fOWwOrcNddkWILRBE5hWRmnDOXusYBEzyaTp2wzGQqFIdLsm2dikH/ZsUkhOgySOA4hG7Zb1cB3x6PhaDiQZRYZkU9GgLY4dli+e3d/XG88uK7K8z1IXuXyjabSHN9zPR1ItCRTt9Xc8u5S9fHicH9Z5nuVxmM6jOLEGo69br8sK/1m75rgCrTSMFyaYrtBZHR7jxfn6+dna3e5/XLx4vJybzpPB+MBz2FT5EY+ShK71yunSSRLgiHLfVXMyeQ+22Ycc3GpvC1g+S8dWeB5VlZ4V1MJktjd2Lm/v2+2mr1ytdKo9KDUblbqrUob+NPD7cr6mqkbkAShROI5rlYupKNRYGlWNpke+qbrm75vO6bmBX48Cpe/rDiGPp0kkyFcogpakXfEINBkmQ5dKfJV27EwrJW4uhfogS9GAZqjdz1OUEmWo4YD5FXJ0pQgcaYle56WJh7LEtnkYDqdjn6zLfjNbAdK+hGcxCh8yXpmFhpsUC9mTr4DVaODwBxO4ySNoO47PV/zfQOyJE5053OILokgwuXBsDy5cbR6f7mzuf3L9dnuUePe82031IfDOAi94TjQHNyPOdPhJvN0Op8+Pc+QRYBG8zwmSTSEw06vkq35RZDWk9QlqY5lcleXuxO0ePtsWSqrigPU8z9Ce5hPT1GcvrVUaAxmNIHpuigrom5IUEm1O02KwvtYD8M6ONbFsQ5BdNvdFlTanz+t8AJrGDJUQGFol8slXZMUWeB4RhJZQWAlJFdKQZUu8oxl6EtLS4APAmuzLG3omu3wACbUfeXLtiFIPBU4Zq1agYgYZDuMukJrMisJTJoE42Fim6plylDwHx3uRYGWydZrRK+z6Nr7ngFnrwa7vxUwWsywZ04EM6DywyFa7x6NozCygX61mwWe6+Su9+bPY6DhUK85riqIGMQwzzMIooH1q7KrFene2u0VExhjeDcQaVEL8oRl+6XSxcnRcu7mIE7RvjL8imWJx4fL66t/Wvny+52td1ivDgTL8dVK8arRuGWYTqN6m7veR/ZBSXC8+6VculUiF6pZAH8QuBCL39ACKTKsm5nRQBUp5C+H1jCR84XMs+NhGPqipjCmziEo+IaqiMBOzs/OqrViFBpI1tGVGaLruqjXxTJFWRLTxA58AbW9o05AEfDx5csXS5dS1HmsAD5830QbzKj0Q72gUaCmkYn1+xpqZiYMlUGrEtlqapi59ETIkxFe0FBVudVoodcPpCQOX+XaFgCCRPn67aJNNHMeRAL/rmMGvsXRBMSwzJN8CMkuTtzRNBlNEmROPs52e1BXFpSXmcbQ0xRJLcSWLBFAwKPAf+RJ0xQq1QtgbHHi3N/t390fbm3+uLf3EQJklPgQk6BgPNj7tLH+57XVH/q94mDgU0z7+ODj+vqPrda9H+hx5FB0fzafUljr8/sfgJeGw3T8NHcccxF33xSwGv6zHE0Flvc9B7iL51mOY9EU47me51quY/u2DefGtg2WZeHWNI13v/7SaSPjExPynetTJAYIiwOVplFMigIDLmzf5T1L6Hdb6xtbaOrGFTy0HAVky/ddYdEOuliyQt3GnqMovO+qSHrZQdkQQlfoKoueiCjbTISyrlou5e9ziswtvKJ/K9u36A79FqKengBncB/4lmWrusqdHW/zLAEca44WT5GZINSAcJsMwtkzmruaITBNZt/6qIhup6SIJMd2dZPzYksPpPv6w22nZJkC6ggdeARRl2Wq3SiQZA2KvoVJmCgSt7dHW1u/XFx+lRWyWnsMAuM2t3d+sQWvMxrF6di7vt4LXeP88mjjZKeqMvYoCUzL8x14b3ESvDWO1ZYd14VwAudYcW3RRqdf8Rx5cRt4mqHzngtlo+baaNJBkaWffvpRUbVms+GYsqGjzRwPESZF00Q7U2YPXD701fu766PDgyjULIN2TMHz0eDGfDaIQwsIVpwZTgGGHBeJfITeN7H4rCVLXrRI+PAGXBW13GT7jKYhnJ4eC4K0MIbIHJyAzU+ztDhBzvSj0SJLou4rpH0K3D0ChC2ek8Ruvf4AROr5ZZxNYmW+lVlnATJTHYW53MH65o+FzKGO5/tRZMkKvb31bn9/JV+7S5CphAPEMQjs89Otg/1PZ6drYajwIu64ChDHk9ON3O3h0fH6Y/4c3r/vq6bCuL41HEKM9GWJLOUuvCTuKyLQQLgIxuh2/oTIH1DDt8axXigr9Fy0i7dYnFy4Ii4GYOBWEQmo3RYaQ2i6y9MYGinAHJ0cAaNKQjUOTeBTY+DLgQzRXlOVJNIhXCWRs7W1SVFUElrwHArvOrY6SEOSwFVZDDwDcBxHWgh/11cYhk5DA94DMt7JjFIW8QzeQPRd4Sj7qQRZ7PbuJojCrCsUsDNZBKoMac+vtobfO9xfMpaFkGYYKkk0bu8OdEOoNQoE3Xt6mY5mQ7RvOAqAL+7vfS7kD1dW/kiSDeD43V5tOAoajYdi4URRcZyodlgMgpzlyoLQ77TyD/eHeztL7VYBaskwsg4Plne3f9ne+unhbv/6+isvYHEapvM4my5E3c+OZ/UFesEEnzIVZXjzwwRpzyR+bGv2WwOW6KW+x3sO2hhmqAYyOe+XsV4JbnGsXK/cQbDJxNYhZkCMUSkSDwN3bW0Nw3rD1IZ002o1r6+u7m9zD/lCPv/4+PBQKuYrldLSh4/lcgXHe3d3d3GcFAp5gsAsy+p2e3EUEwSu65Jr64rMX55furZpW0DVVSDsjq25joGmWx0DDt8zA8/yveyO78VxXKlUMzMItDz6vUd09lfNopl9JoLX7GkwiA1dBkzjeE0Qex4w8Wk8fRoCDU+HkWmJHN8BVrS68kOldHZ/fzyZRBcXO/BTYOUnRxu1ynXuemd7+9fjyt1oErea98eHX7Y2ftnf/QiPX1xu6EijS1/+/H92995tbP5UKJzsbC/d4fV7lTcjF0rUYZrEXmRoaJpQkXSRFMS+yLV5sSvJfYXrsHiFoOvMWwNWV/eQMJonGxqlKcgV8bXtCcBEE81Os5C1Ri128TQc76eJ3+t237//eHZ6vLqyvL2LvnZ2do7R1xGchIOD/b293U+fPh0dH+/t7bXaXcie9Tqahy6Vij/99DMgrNPpADpTtAVjxrEbhUDmdACTjboF5cVh6IqGhEYFWUJmGXAHDpHnHvJ3UQhZBc4TDWRwgagFo1+sl353NkTjzjjWhldO48h2VKQg+fw0nqZAugsPV8Dr282H68s9ge3t7Xw4PV7rdkumyRF4NYCnewaUcre5fbSNs/bDwelmq/2wDPeXfwDmvvN16WD30+7Ox9EgZknyYGv1fH+r/nDffCxSbYxp0xIm9Rsk02TIGlm/qdZz9VquWr4uAYaoMkmUia8fv/7pX/78z//9n5Z/Xu4+9t8ax6rrqY+snVVLo0yDsT3B9VHS8W1gU1S9+hBAigzQ+hOkLWA8zXpd5AWGouvV+p9++KlcLk0mg16vpeuaJAuDQeQ5buDqDEMcHB5RDEEzTD6fhzhSqdQGg8Hnz5/++Z9+R5GkKHDv370fDULX4Rdpzv/eUJqtRyjoCJRvd7IDyYegW2U8DhmGRcqRtjwaRq+xCmj7DPlkzhdKRmmaziYDiacdxxgg1bUks0JBwtm18u3p0VeO7p4erpUec6bFV0rnnW4hiHSW6zJcczKNSLJVfry4PN/d3Vg63F67q1W7jWr9/rby8NDMl6kWQTUJsk5KfYHrcGSNIms0Vaevdq82P2z+w3/+hz/+7ofOAwYP0vDg3lXpulLJ1Ru3TaJM4mWcaVC9ch+rUXSHUSnFYNU3BSw0QujOgBJ5cF4twdHoxFcSTxnELlSFwChpqhsGkhdokbuIWPIgtpPISmM7jd1ep7a2jiQksV5nlAaGJogCY0JOM7VqpXhzkwPuEgWmqSt721+bzSrwinz+Ln9/i9x4bL1eKw9iC8Ik2ijMGthf/Z5+e3wb8/rNT11H0XU1CA2IrwCsKAzjOERatplt3SvHylLhJPCczMRkMkOsbBoELkNj5dL1IA4oogfRdzwZubaBdWosjosUK+AU36dEXCDqGFXrc22WqVNYqV++apxtXTyeFvmGKPU1qsEerp/sLu8erR1C5GKbPNcUsMf+/XG+kqtxXZ7rckIfninxPdHkbZ3VbMlwFNs3vUGQplEi6Xo8R4twqio8Pl6/tQXSfvA8jJA6o6ISLaJ+26vWeZISOYxjCIWptwuegbku5/twoLOL2LQvRgty7SM9yJ2dreUvn3GsAzCKI5emIRrxudw1y8Kv6CESNbXCQA4iaTT0o8jMIpMOhZ5tsZZBZT3s6ituFv19vwXTvwcW6oAw4Lxk9j62ZOqCoQsLu8rXPcRvNtJo6AqVhMjxdzofxHG33iBa3W6lrjE60yWFnggnXsFVrEIwTaH7gB+tnZ5tHe+v7p1/PeGbDFNnOvftg5WDYi5fLVXy9/edRit3eXNxctaqNwRSIJo026aFPov1KEe2YyuMHX8QJePBaAgEPnTnaKxjlmXrbwayUAzGsWcbymQ25QHTFB573luLWHVnrnlOk8ILRK8vspzM9gX8GqveEeUiV7siHwWbCiFcQbGGqBjkRBS3Mo0GtEUT+6gY7GH91dW11dUViux5jpbEfr/fPTs9S5OwXCpiWNeyJKDJ6+sr8MGapkhTBEcDpWMh2Rka4zvKwuNpMdGFakBX/uvkmAEr+AasJJSj0GDRtJlhm9xC02EyHo+Ho+logs5oAmQ98UzXVV2ZVPguz7SYfgXvFTGywtA1FohOr0jcHORON0/W3q/98Ls/si2ITHT/sV+6LjXynU6x3y8TdIMF5NEtTuHUi/PrL5+Wv25u7ux8PT4/vbq+WllZ/rq7q7v2/Hl8Vzg77RRnL8/Tl0nh8UqRadNU/MAMXctzTMe2giB4ypr6HLgOWDoTTvaSNBzC251M4Pp7W8BC9kzPdT3kdS6O9DDQBR5zPU6zWrrVMcyuYvcKbKXH9TpS37GlUWrEHpuGmu3pgs51+BZjUbGvAomBIlFRtI2N1eWVzyTeF0URItbvf//7TqcNxCuzCcH/9m//9ulpXq1W9/f3oTz8x3/8R0WR7+8eoAhIYohk4kLmFBnmQKHqC2GkLOZX41CFsIoaRw3BlBiV5iWC1UWtXmtqFC/0aKFLsx0OACT2BKbNioSOVej6XZeqM0yTA3b80//+aW9l+3LvvFWo08gZm8WKWCvf6j70+8U+12KxcrtyW+iWm4NgkL+8LNxcR64n8+L1+c3RwdHnz1+ub65c18kWzKbZKCzUm4O11ZVWtwGR+OxqpyBi89koX7k52V1VJYalMVlmAqRXCE9GTojT8RgqEt+zIIZBWQEFDIW3fcccjUeYqb61qrCaedDdtm4b/fNi6zhX3c7Xt0nh3tD7rkP7Du+GqmSwrMGVBfyR6fVUrqtS92SpLNcJtVVky7iCBS5EKZsmyck4bTZLG5ub735dWl9fRza3LAUf7tHRQavV/pu/+Q+maWIYjuG4aRj/63/8z+urK/hY//7v/9PL0wiZXDqqayi2KtuKYvKySgsqwcsYx7Voss7wXeArstCV6TpLV9lupV8uVUpX5Zu92+Z9h23wEIoezx633m+9/9d373//rnT1CACiazTXFmRs0eXFs21WwlmV4QxBCkzf1k2ZZwWmf7C3srn2K8/1OB6/uT4qFHJbm+vLK6uXVxcQnOr1epbEXqazcRQ7msoGgYH1a1dXZ6srqxSBF3oVPfXG82GrXcA7ZdtU48iHKypJoul0krm2zoAIDtIEDTlmayG+ZwceWj4FjCaT8VsDVs2bQUK8ZxqS0uGUdr1/89g5pqVKgHZdIEPJqYcGkSOU/mTIho4voBF4H6JaT3dw26YqfF82IU8ppqHalhJkkh4sg+1sb1vA401zNBpeXl4enxz/3X/8Ozg3F5cXpxdnULL98vPPaRR93f7a7fQMXmOaJFUn2RYAiIf8pdCqSssCrYiMJHGiKIiZg00LQqOpmHSVxqv4w91Du9jsPEKCI6kqzTRoukmyHVLEWJmEKCwZAu/pgFQBIBs6iqWRgYOIHdoVDVVVlaHGaDcfGbKVuz5o1u67nXqtWlrf2Nre3nUcNxPJHX/3O8mcVJ8mN7eH/X7p4HCl1b6zbaXT733+9GVzb9c1TXcIzEmeTgZP3y2onp/nr9bUz992C+aLpgvfc4G2T5BcDxq0fWvAWkg2XJN4j7zn1c5D49izuYWKFeI3aK5LyZiNlPha5EHNqAUeEgWN/WzfGlh5oEFBpTuiaYi6jr71XMN1TNdzur3u1tamKIvjwTg2hxN/JBMS6kaWXVMwZUKp3lZlVrZN21ANB2BomCrEKZJu1FvNRouiGFVQHc02ZUthZI1RZUI0BEVTFVORdUmiCVIRFbhja4JtQbWIVlYD3w1D37YhJmpRhIZpLZNKQs20JZZmVVnUFBouA9tksjEhxrUUx1axXrVUzJG99srKSrVW+003zixN48EwzuyZoCjZq9duHFdL0xC1yc8GQWTHsZ/P5798/gIsc31tfXVtDV7k+Pj49OQ0/5CHpB/Hyet20/eXRQ1k38x/4Nv5/E0C6zkvGPnyDq92i83rCKmJInKD9oORhNA3SQ/X4U2dDVxlAFWerbiapPKUzsoixjAES3YIjuA8N3BtezwY+m4QePFwMOE47t27dzTJ/PqHX//bf/mvP//Lj91Cj6pRbJMrXRRLF+V6oW5rFtkjmR7D9Vm2R0sk8DfZQ0bpqqNLrgGH6KNEiY7QVyvVMo71Oq06y7EMRyG7VySSbIehZ9uGYUge6pJQXVsXOcCQFYXO+fnZxw+f9na2P3/6tPJl9eDg6PT09PLi8vzk8Oz0FCBwfnh4dLC7sbl+dnKG1uz/EnKeMxPoGMMr+Ydj3eSHowRJJiMXwqEfWpNJJCv0/AkqTk8QWVUTdUNRVCELs2yv11tdXX3//v3nz5+AWZ6dnQHdhHcLrxyGqE3DdW0A7tPT7I1VhWiavuo+1ezJdXmb1zv39T1BqnuW6FnojJoykGVBFwSN5aU+zXUIAW67LNvlqCZNNZlutd2v9ZqleqfSKj6Urq6u9va3P35eWvq0tAIf6If39WqDY/iV1S8MzmusKfQFpkXzPY5pU0KfUUjg4AKJE6ivQpUCW0kDpCgZZAsQqAnHzXQAs93DhdhaHBrdbud5PhwNAl3XVEmI0Ba1lIR65C80cCVT51WZZWkc4l6x8PDly8fzi1NV4QcDG4o1jqXv72+ury+BP13fXF7f3ubv7q9vc2dXlziOv/bkLDpwskX8aRS7z4tH0KApHFM0pD9CTfRPz9N0iFxVRYVOh+FTpgUCx2iSGqYCwQiYAORTyLuNRuP+/n5vbw/i2dLSEiBbURQgXvCc6XT81pYbmi5Sw0IaIa2CSHL9epdqUVyHpRsU3xVlSuMxke1zIiGWrkqbnzaxLg50dXnly8dPH1ZWP29tbe/uHu7vHeaubhu1RrfTNzVL4SQWo7g+pbDi9tedo/2j1fXVYvGx1+64thHbqqUzjskYOu5ZLDC2JPUJknQdUdfglguzVlLbYBedDkD1Ak/zMilA4EYOcjhXIx8JdBu6BNTL87Rup31xeQlFw8bm1vLq6tLHj2urq5sb69vbX+9uc7rK+RZqJOR5Oo3DyXzcE5mbRrncrD2qfF7k+qbR1qVoOs5c4+bPv/mCmFWrFerNx8nzaIK6IdA4/HDqxyPNT4VwoMRDO0qdySyJB94sGxUcZzp9E2RwP4vQArGBvGi/59bFQm6SJI+Pj1tbW6+KcG8tFWbT9M9IBb7cxlsMUezent5BempWG/e3t8vLK8vLqysra0tLH/f39mmc2VjbuL66ViTV1W1DhCjAmorkAZ+RJV0S4XBdCHO8rkgcjeP9duEhv/xl+fPnz2EU565vZFnRoOR2IcboYehA+EF3ApPjKM+WXItVJMw1OFOjVBFD3ieLVotg0ewAFEoLfNtzLceVgfG0ISI16nt7O58+fT49PWrUK91+Q1H4wDOR2Q7qZkZdN74N6YlJYncwHImDuCULzmAQDkYBWjtFLRDpEHXEm6aeneP5eDxcKCBNpuPz051a5XY2G6H50mc0BDuZD4KhbqeslcBBBUNZUihV48PImWTWvdkA2XAwDKaTqefZgKq/9ub85gELpA0+Ftd1FzubbxBYi9DVZQzH8IkeoiyKJG9ub19enTMkJnOMIQuWLkNpDUThl1/f4XgvjewkQpK1A9RpyUAlVS1XKYrG+zjHy5IgCCwJKc40JFWRKuXyx48fh0naMyUeEETTUIdDOgNWW6tXMAxzbct1FxiSM6kZARAWoUZkpDsSI9VTKY4sUWRYhirkC3d3eQhFnz59qlaqkOnarXqxWIRUGPsq8oTKBqxNk1D4tiX1fZsfD0NdVRhNIz0rGU2g9kc9FAb/9IQWAnwPdVL4nqZp7Aj5SS+EkFDia9QfD3dWsgw4e3pBqpCQ+6LUDVLL8oUgVWWjX6xcajqHLJmQHPfCpWJM0q2bqz3LVgaDZD6fPP/112ujIhTL8O8vvn2bwIKjY8YCS6mqZuu8pcofPyzZBmqHkvhu4IqOyY5TjyChtP5Uq9Yuzi8fi+VuD2/Ua1dXuXa7AakqRrs3iudono1kt7PMJYehDRxrZ2c7d3PTYHBClwBYN9c3SDs+DSeTJE2dxYx/jNZIoZzUIMGZpmoYChymoVqQXKFq4Fie5xiGUhUBCAqAKY1tqFjh71qWdHd3l0A1aguBA9cArWuEYzGMQOAayxoSVIyQVjuaIKsi8KHxZKjJzCgwLEtWNah8JdtEyvLwUtPJ8HsTzhPH4Cuf/wRpdDadpkkgKuwAuUcjCSQ/siWZCSPz5maX49rdbokg6patonmybE6fZdqP98dJEiHniyxi/RtgLTgchKsPHz4s9qDeLLCoIOl0u0ngIBEiR1xZWcaxzkIBZqFVDCADqry9vTMaej7gJtQcS3BtJPgO59tEfaRGGECVrxq66ntW1koFRZrW6bSvri4t2z47OZU0meIZXhBojqFJwkU2AnbgKY4FT+uIApK+jUIfDSf6Nhw8xyKbOkcncRIqviiyAFgfPvw6HoW+g8QgTB1omXx5ccpwrKmSqkQYOgZPZwWqKPQcRxEMrs9hlq3HkHM9edEika2koL5nVaYDVB8oEo8hFTjkHzEPAzeN/Xzu5Gh3OY482zKzwdjhKPPPGU1j3RBnT3M/0E9PNjrdoiARpiOHsZdNVw9tV6XptmnwgzTN0tz8u67uv/GrRnwL8vjW1mar1XqzwKKDpNvDAFgZm1G3tr52Oo3oe0OprpKhL0Hl9uuv7yErRcG3NuIIVWFKGJoR1G8iD6lKkYGDIwjCLeRByHeQIoGoiqIIlRGU/QxJUqZcx3u2psMj1Xo5SUKOF+AJkBe63Xav2+E4ZjhIozCA6KVrIkSyKHLCUPN8CGby0vsPw9TVJIwlG47BWAYTB9bNZU7X0eYjcl511VuumXq6DwAKkDyJ7yv/z76JyNciZGugOCYSfNMNzvMcqNF0XTo72RG4Hs/iqoJ20x0H3oYZhtbZxZbnKZalnJ9uVSq5MDRUlbMdbZDJHQ6HoR+aSepAWoR6cJq18bwi6RVVr0QecmWh8PD169c3y7E4P8UJtOoToO05GYDValU9NEuDxPsVEQs90bT49fX1VrMOZxwglXEaIetjRs1S2YgE8rdZWLCiRvhAo0ii1+21WrWbbK+tVqsB3z47P1c9uylQhq64lqorPJ95XgLngI8YUMgwjG3ZURRBbkLTHJY2mYyiyEeOhHF8fXkFDDtNIK44gaMMEmcySuB86gaUigKlUG2FinzdD7KGiG8Wr9L/ryEnuzyUxZ1M6EYZpAEUFs3mo2kiBzy05eDxfsArMtVuF6+udzqdx92vH0ms0m48wINwXSkqTTFtjsdopm9aygiSZqb58bpy8e8jFtwGQQDAyhqq5/8X3huKFu7I7SMAAAAASUVORK5CYII=";

      Image im = null;
      try
      {
        byte[] array = Convert.FromBase64String(image);
        im = Image.FromStream(new MemoryStream(array));
      }
      catch { }
   
      return im;
    }
  }
}