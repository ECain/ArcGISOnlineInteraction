/*
 * Author: Edan Cain, ESRI, 380 New York Street, Redlands, California, 92373, USA. Email: ecain@esri.com Tel: 1-909-793-2853
 * 
 * Code demonstrates how to structure REST calls for interaction with ArcGIS.com organization accounts.
 * Calls are made via HttpWebRequests based on string descriptors. Both GET and POST calls are within this code 
 * and response format is JSON.
 * 
 * HttpWebResponses are dynamically binded too via the DataContract objects found within the AGOLRestHandler project.
 * 
 * Code is not supported by ESRI inc, there are no use restrictions, you are free to distribute, modify and use this code.
 * Enhancement or functional code requests should be sent to Edan Cain, ecain@esri.com. 
 * 
 * Code only supports Feature Services / Map Services.
 * Feature Services can be queried, edited. Also, code allows for the dynamic creation of Feature Services based on the attribute
 * table structure of another feature service. 
 * For code to allow for the dynamic creation of a feature service attribution table, contact the author, or adjust the code in
 * the click event btnAddDefinitionToLayer_Click.
 * 
 * Code created to help support the Start-up community by the ESRI Emerging Business Team. If you are a start up company,
 * please contact Myles Sutherland at msutherland@esri.com.
 */

using AGOLRestHandler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace AGORestCallTestFS
{
  public partial class Form1 : Form
  {
    Dictionary<string, FeatureLayerAttributes> _featureServiceAttributesDataDictionary;
    Dictionary<string, string> _featureServiceRequestAndResponse; //key value pair will have a $ delimiter for the request string then the response 
    Dictionary<string, Item> _myOrganizationalContent;

    FeatureEditsResponse _featureEditResponse;
    FeatureLayerAttributes _featureLayerAttributes;
    FeatureServiceInfo _featureServiceInfo;
    FeatureServiceCreationResponse _featureServiceCreationResponse;
    System.Web.Script.Serialization.JavaScriptSerializer _javaScriptSerializer;

    string _baseApplyEditsURL = string.Empty;
    string _token = string.Empty;
    string _organizationID = string.Empty;
    
    DataTable _dataTable;
    DataRow _editedDataRow;

    string _myOrgServicesEndPoint = string.Empty;

    enum EditType
    {
      add,
      delete,
      update
    }

    public Form1()
    {
      InitializeComponent();

      //JavaScriptSerializer is used to deserialize httpResponse JSON based on the DataContract provided to it.
      _javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

      _dataTable = new DataTable();
      dataGridView1.DataSource = _dataTable;
      _dataTable.RowDeleting += _dataTable_RowDeleting;
      _dataTable.RowChanged += _dataTable_RowChanged;
    }

    #region Functions

    /// <summary>
    /// Display Map thumbnails from the user's organization account
    /// </summary>
    /// <param name="files"></param>
    private void LoadImages(List<Image> files)
    {
      try
      {
        if (files == null)
          return;

        //Clear the current content if any within the DataGridView
        dataViewImages.Rows.Clear();
        dataViewImages.Columns.Clear();

        //Display height and width
        int imageWidth = files[0].Size.Width;
        int imageHeight = files[0].Size.Height;

        //Number of columns and rows required.
        int numColumnsForWidth = (dataViewImages.Width - 10) / (imageWidth + 20);
        int numImagesRequired = files.Count;
        int numRows = numImagesRequired / numColumnsForWidth;

        //Do we have a an overfill for a row
        if (numImagesRequired % numColumnsForWidth > 0)
          numRows += 1;

        //Catch when we have less images than the maximum number of columns for the DataGridView width
        if (numImagesRequired < numColumnsForWidth)
          numColumnsForWidth = numImagesRequired;

        int numGeneratedCells = numRows * numColumnsForWidth;

        //Dynamically create the columns
        for (int index = 0; index < numColumnsForWidth; index++)
        {
          DataGridViewImageColumn dataGridViewColumn = new DataGridViewImageColumn();
          dataViewImages.Columns.Add(dataGridViewColumn);
          dataViewImages.Columns[index].Width = imageWidth + 20;
        }

        //Create extra rows
        if (dataViewImages.Rows.Count != numRows)
        {
          for (int index = 1; index < numRows; index++)
          {
            dataViewImages.Rows.Add();
            dataViewImages.Rows[index].Height = imageHeight + 20;
          }
        }

        dataViewImages.Rows[0].Height = imageHeight + 20;

        int columnIndex = 0;
        int rowIndex = 0;

        for (int x = 0; x < files.Count; x++)
        {
          // Load the image from the file and add to the DataGridView
          Image image = files[x];
          dataViewImages.Rows[rowIndex].Cells[columnIndex].Value = image;
          string[] split = files[x].Tag.ToString().Split(',');
          string webmap = split[2] + ": ";

          dataViewImages.Rows[rowIndex].Cells[columnIndex].ToolTipText = webmap + split[1];

          // Have we reached the end column? if so then start on the next row
          if (columnIndex == numColumnsForWidth - 1)
          {
            rowIndex++;
            columnIndex = 0;
          }
          else
            columnIndex++;
        }

        // Blank the unused cells
        if (numGeneratedCells > numImagesRequired)
        {
          for (int index = 0; index < numGeneratedCells - numImagesRequired; index++)
          {
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle();
            dataGridViewCellStyle.NullValue = null;
            dataGridViewCellStyle.Tag = "BLANK";
            dataViewImages.Rows[rowIndex].Cells[columnIndex + index].Style = dataGridViewCellStyle;
          }
        }
        dataViewImages.CellContentDoubleClick += dataViewImages_CellContentDoubleClick;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    /// <summary>
    /// Populate the Feature Service Field list with field names
    /// </summary>
    /// <param name="fields"></param>
    private void PopulateFieldsList(object[] fields)
    {
      //Clear all previous items
      cboFieldNames.Items.Clear();
      cboFieldNamesQueries.Items.Clear();
      chkbxReturnFields.Items.Clear();

      Dictionary<string, object> dict;

      _dataTable.Rows.Clear();
      _dataTable.Columns.Clear();

      Type type;
      foreach (var item in fields)
      {
        dict = item as Dictionary<string, object>;
        string name = dict["name"].ToString();

        type = FieldType(dict["type"].ToString());
        if (type == null || name == "FID")
          continue;

        _dataTable.Columns.Add(name, type);
        cboFieldNames.Items.Add(name);
        cboFieldNamesQueries.Items.Add(name);
        chkbxReturnFields.Items.Add(name);
      }
    }

    /// <summary>
    /// Limited FieldType returns. Extend if you want to use the other types
    /// </summary>
    /// <param name="esriFieldType"></param>
    /// <returns></returns>
    private Type FieldType(string esriFieldType)
    {
      Type type = null;
      switch (esriFieldType)
      {
        case "esriFieldTypeDate":
          type = typeof(DateTime);
          break;
        case "esriFieldTypeInteger":
          type = typeof(int);
          break;
        case "esriFieldTypeSingle":
          type = typeof(Single);
          break;
        case "esriFieldTypeDouble":
          type = typeof(double);
          break;
        case "esriFieldTypeString":
          type = typeof(string);
          break;
        case "esriFieldTypeOID": //not required for project
          break;
        case "esriFieldTypeGeometry":
          break;
        case "esriFieldTypeBlob":
          break;
        case "esriFieldTypeRaster":
          break;
        case "esriFieldTypeGUID":
          break;
        case "esriFieldTypeGlobalID":
          break;
        default:
          break;
      }

      return type;
    }

    private void PopulateExtentValues(Extent extent)
    {
      txtXmax.Text = extent.xmax.ToString();
      txtYmax.Text = extent.ymax.ToString();
      txtXmin.Text = extent.xmin.ToString();
      txtYmin.Text = extent.ymin.ToString();
    }

    /// <summary>
    /// Display Feature Service default symbol
    /// </summary>
    /// <param name="obj"></param>
    private void ShowSymbol(FeatureLayerAttributes obj)
    {
      if (obj != null)
      {
        if (obj.drawingInfo != null)
        {
          if (obj.drawingInfo.renderer.symbol != null)
          {
            if (obj.drawingInfo.renderer.symbol.imageData != null)
            {
              //within the JSON response is the serialized image data, use the provided helper function to deserialize it
              System.Drawing.Image image = RequestAndResponseHandler.DeserializeImage(obj.drawingInfo.renderer.symbol.imageData);
              pictureBox1.Image = image;
            }
            else
              pictureBox1.Image = null;
          }
          else
            pictureBox1.Image = null;
        }
        else
          pictureBox1.Image = null;
      }
    }

    /// <summary>
    /// Display to the user the requests and response that are produced
    /// </summary>
    /// <param name="formattedRequest"></param>
    /// <param name="jsonResponse"></param>
    private void ShowRequestResponseStrings(string formattedRequest, string jsonResponse)
    {
      txtRequest.Text = formattedRequest;
      txtResponse.Text = jsonResponse;
    }

    private void EnableControls(bool enable)
    {
      listBox1.Enabled = cboFieldNames.Enabled = cboFieldNamesQueries.Enabled = cboClause.Enabled = cboGeometryType.Enabled = cboSpatialRelationship.Enabled = enable;
      txtXmax.Enabled = txtXmin.Enabled = txtQueryString.Enabled = btnQuery.Enabled = txtYmax.Enabled = txtYmin.Enabled = chkboxReturnGeometry.Enabled = chkbxReturnFields.Enabled = enable;
    }

    private void EnableEditingButtons(string capabilities)
    {
      if (capabilities != null)
      {
        btnAdd.Enabled = capabilities.Contains("Create") ? true : false;
        btnDelete.Enabled = capabilities.Contains("Delete") ? true : false;
        btnUpdate.Enabled = capabilities.Contains("Update") ? true : false;
        grpBoxQueries.Enabled = capabilities.Contains("Query") ? true : false;
      }
      else
      {
        btnAdd.Enabled = false;
        btnDelete.Enabled = false;
        btnUpdate.Enabled = false;
        grpBoxQueries.Enabled = false;
      }
    }

    /// <summary>
    /// Ensure the Service URL is correct
    /// </summary>
    /// <returns></returns>
    private string CompleteServiceUrl()
    {
      if (!_baseApplyEditsURL.EndsWith("/"))
        _baseApplyEditsURL += "/";

      ListViewItem item = listBox1.SelectedItem as ListViewItem; 
      return string.Format(_baseApplyEditsURL + item.Text + "/FeatureServer/0/applyEdits?f=pjson&token=" + _token); 
    }

    /// <summary>
    /// /// This operation adds, updates and deletes features to the associated feature layer or table in a single call (POST only). 
    /// The apply edits operation is performed on a feature service layer resource. The results of this operation are 3 arrays of edit results (for adds, updates, and deletes respectively). 
    /// Each edit result identifies a single feature and indicates if the edits were successful or not. If not, it also includes an error code and an error description.  
    /// </summary>
    /// <param name="type"></param>
    private void EditFeatureService(EditType type)
    {
      string formattedRequest = string.Empty;
      string jsonResponse = string.Empty;
      string jsonToSend = string.Empty;
      string attributes = string.Empty;

      string url = CompleteServiceUrl();

      //Create the HttpRequest based on Edit type
      switch (type)
      {
        case EditType.add:
          {
            if (_dataTable.Rows.Count == 0)
              return;

            object[] row = _dataTable.Rows[_dataTable.Rows.Count - 1].ItemArray;
            attributes = "\"attributes\":{\"";
            int counter = 0;
            foreach (DataColumn column in _dataTable.Columns)
            {
              attributes += string.Format("{0}\":\"{1}\",\"", column.ColumnName, row[counter].ToString());
              counter++;
            }

            attributes = attributes.Remove(attributes.Length - 2, 2);

            //NB: Sample does not provide an entry method for the user to enter an XY for the Geom value. 
            //this is for demo purposes with a point feature service with the spatial ref as below.
            //Users of this code need to build this functionality into their app, either with text input or map click.
            //Supplied XY places a point on the Coronado Bridge, San Diego, California. Will work only for Point feature types.
            jsonToSend = "adds=[{\"geometry\":{\"x\":-13041634.9497585,\"y\":3853952.46755234,\"spatialReference\":{\"wkid\":102100, \"latestWkid\" = 3857}}," + attributes + "}}]";
            break;
          }
        case EditType.delete:
          {
            try
            {
              jsonToSend = string.Format("deletes={0}", GetSelectedRowFID());
              _featureEditResponse = RequestAndResponseHandler.FeatureEditRequest(url, jsonToSend, out jsonResponse);
            }
            catch { }
            break;
          }
        case EditType.update:
          {
            jsonToSend = string.Format("updates=[{\"geometry\":{\"x\":-13041634.9497585,\"y\":3856052.46755234,\"spatialReference\":{\"wkid\":102100, \"latestWkid\" = 3857}},{0}", EditRow());

            break;
          }
        default:
          break;
      }

      //Make the request and display response success
      _featureEditResponse = RequestAndResponseHandler.FeatureEditRequest(url, jsonToSend, out jsonResponse);

      //Display success of operation
      switch (type)
      {
        case EditType.add:
          {
            if (_featureEditResponse.addResults == null)
              break;

            lblEditingResponse.Text = string.Format("Success: {0}, ObjectID: {1}, GlobalID: {2}, Error: {3}", _featureEditResponse.addResults[0].success,
              _featureEditResponse.addResults[0].objectId, _featureEditResponse.addResults[0].globalId, _featureEditResponse.addResults[0].error);

            if (_featureEditResponse.addResults[0] != null)
            {
              for (int i = 0; i < dataGridView1.ColumnCount; i++)
              {
                if (_editedDataRow.Table.Columns[i].ColumnName.ToLower() == "fid")
                {
                  dataGridView1.Rows[dataGridView1.Rows.Count - 2].Cells[_editedDataRow.Table.Columns[i].ColumnName].Value = _featureEditResponse.addResults[0].objectId;
                  break;
                }
              }
            }

            break;
          }
        case EditType.delete:
          {
            if (_featureEditResponse.deleteResults == null)
              break;

            lblEditingResponse.Text = string.Format("Success: {0}, ObjectID: {1}, GlobalID: {2}, Error: {3}", _featureEditResponse.deleteResults[0].success,
              _featureEditResponse.deleteResults[0].objectId, _featureEditResponse.deleteResults[0].globalId,_featureEditResponse.deleteResults[0].error);

            break;
          }
        case EditType.update:
          {
            if (_featureEditResponse.updateResults == null)
              break;

            lblEditingResponse.Text = string.Format("Success: {0}, ObjectID: {1}, GlobalID: {2}, Error: ", _featureEditResponse.updateResults[0].success,
              _featureEditResponse.updateResults[0].objectId,_featureEditResponse.updateResults[0].globalId, _featureEditResponse.updateResults[0].error);

            break;
          }
        default:
          break;
      }

      //Display request and response JSON to the user
      formattedRequest = string.Format("Request type: POST, URL: {0}, Content: {1}", url, jsonToSend);
      ShowRequestResponseStrings(formattedRequest, jsonResponse);
    }

    private string EditRow()
    {
      string attributes = string.Empty;
      if (_editedDataRow != null)
      {
        attributes = "\"attributes\":{";
        for (int i = 0; i < dataGridView1.ColumnCount; i++)
        {
          attributes += string.Format("\"{0}\":\"{1}\",", _editedDataRow.Table.Columns[i].ColumnName, _editedDataRow.ItemArray[i].ToString());
        }

        attributes = attributes.Remove(attributes.Length - 1, 1);
        attributes += "}}]";
      }
      return attributes;
    }

    private int GetSelectedRowFID()
    {
      int value = -1;

      for (int i = 0; 0 < dataGridView1.ColumnCount; i++)
      {
        if (_editedDataRow.Table.Columns[i].ColumnName.ToLower() == "fid")
        {
          value = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[i].Value);
          dataGridView1.Rows.Remove(dataGridView1.SelectedRows[0]);

          break;
        }
      }
      return value;
    }

    private void UpdateMe()
    {
      string request = string.Format("{0}sharing/content/users/edan.cain/items/{1}/update?f=json", txtOrgURL.Text, _featureServiceCreationResponse.ItemId);

      string payload = string.Format("title={0}&description=test Description&tags=Hello World&extent=-134.747,16.475,-55.696,57.395&thumbnailURL&http://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/export?size=200,133&bboxSR=4326&format=png24&f=image&bbox=-134.747,16.475,-55.696,57.395&typeKeywords=Data,Service,Feature Service,ArcGIS Server,Feature Access,Hosted Service&f=json&token={1}", _featureServiceCreationResponse.Name, _token);

      //feature service
      string request2 = string.Format("http://services1.arcgis.com/{0}/arcgis/admin/services/{1}.FeatureServer?f=json&token={2}", _organizationID, _featureServiceCreationResponse.Name, _token);
      HttpWebResponse response = RequestAndResponseHandler.HttpWebGetRequest(request2, "");
      string JSON = RequestAndResponseHandler.DeserializeResponse(response.GetResponseStream());

      //NB: to check the status of the call uncomment this. Add a thread wait and check again until the status shows completed.
      //http://services1.arcgis.com/{0}/arcgis/admin/services/{1}.FeatureServer/status?f=json&token={2}", _organizationID, _featureServiceCreationResponse.Name, _token);

      //relatedItems
      string relatedItems = string.Format("http://ebgtest.maps.arcgis.com/sharing/content/items/{0}/relatedItems?relationshipType=Service2Service&direction=forward&f=json&token={1}", _featureServiceCreationResponse.ItemId, _token);
      HttpWebResponse response2 = RequestAndResponseHandler.HttpWebGetRequest(relatedItems, "http://ebgtest.maps.arcgis.com/home/item.html?id=" + _featureServiceCreationResponse.ItemId);
      string JSON2 = RequestAndResponseHandler.DeserializeResponse(response2.GetResponseStream());
    }

    #endregion

    #region UI Control -> Events

    private void _dataTable_RowChanged(object sender, DataRowChangeEventArgs e)
    {
      _editedDataRow = e.Row;
    }

    private void _dataTable_RowDeleting(object sender, DataRowChangeEventArgs e)
    {
      _editedDataRow = e.Row;
    }

    private void txtFeatureServices_TextChanged(object sender, EventArgs e)
    {
      _baseApplyEditsURL = txtFeatureServices.Text;
      btnConnect.Enabled = true;
      listBox1.Items.Clear();
      cboFieldNames.Items.Clear();
      pictureBox1.Image = null;
      txtSupportedFunctions.Text = string.Empty;
      cboFieldNamesQueries.Items.Clear();
      chkbxReturnFields.Items.Clear();
      txtXmax.Text = txtXmin.Text = txtYmin.Text = txtYmax.Text = string.Empty;
      EnableControls(false);
    }

    private void txtMyOrgServices_TextChanged(object sender, EventArgs e)
    {
      btnOrganizationPersonalContent.Enabled = true;
      listBox1.Items.Clear();
      cboFieldNames.Items.Clear();
      pictureBox1.Image = null;
      txtSupportedFunctions.Text = "";
      cboFieldNamesQueries.Items.Clear();
      chkbxReturnFields.Items.Clear();
      txtXmax.Text = txtXmin.Text = txtYmin.Text = txtYmax.Text = string.Empty;
      EnableControls(false);
    }

    private void cboGeometryType_SelectedIndexChanged(object sender, EventArgs e)
    {
      string value = cboGeometryType.Text;
      
      //Removed everything except Envelope for simplicity
      switch (value)
      {
        case "MultiPoint":
        case "Point":
          txtXmin.Visible = false;
          txtYmin.Visible = false;
          lblXmin.Visible = false;
          lblYmin.Visible = false;
          lblXmax.Text = "Longitude:";
          lblYmax.Text = "Latitude:";
          break;
        case "Polyline":
        case "Polygon":
        case "Envelope":
          txtXmin.Visible = true;
          txtYmin.Visible = true;
          lblXmin.Visible = true;
          lblYmin.Visible = true;
          lblXmax.Text = "xmax:";
          lblYmax.Text = "ymax:";
          break;
        default:
          break;
      }
    }

    /// <summary>
    /// Contains all of your AGOL user layers
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      ListViewItem item = listBox1.SelectedItem as ListViewItem;
      _featureServiceAttributesDataDictionary.TryGetValue(item.Text, out _featureLayerAttributes);
      _featureServiceInfo = item.Tag as FeatureServiceInfo;
    
      //supported functionality
      txtSupportedFunctions.Text = _featureLayerAttributes.capabilities;
      EnableEditingButtons(_featureLayerAttributes.capabilities);

      //feature service fields
      if (_featureLayerAttributes.fields != null)
        PopulateFieldsList(_featureLayerAttributes.fields);

      //extent of feature layer.
      if (_featureLayerAttributes.extent != null)
        PopulateExtentValues(_featureLayerAttributes.extent);

      //display the default symbol
      ShowSymbol(_featureLayerAttributes);
      lblSelectedFS.Text = "FS: " + item.Text;

      string value = string.Empty;
      _featureServiceRequestAndResponse.TryGetValue(item.Text, out value);
      if (value.Length > 0)
      {
        string[] split = value.Split('$');
        if(split.Length == 2)
          ShowRequestResponseStrings(split[0], split[1]);
      }
    }

    /// <summary>
    /// Displays the info about the selected Item to the user
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cboItems_SelectedIndexChanged(object sender, EventArgs e)
    {
      txtMyServices.Text = string.Empty;
      txtMyOrgServices.Refresh();
      Item item;
      if (_myOrganizationalContent.TryGetValue(cboItems.Text, out item))
      {
        lblSelectedFS.Text = string.Format("FS: {0}", item.title);

        //long handed (no string.Format) for clarity
        //
        txtMyServices.Text += "Title: " + item.title + "\r\n";
        txtMyServices.Text += "Type: " + item.type + "\r\n";
        txtMyServices.Text += "Owner: " + item.owner + "\r\n";
        txtMyServices.Text += "No# Ratings: " + item.numRatings + "\r\n";
        txtMyServices.Text += "No# Views: " + item.numViews + "\r\n";
        txtMyServices.Text += "Last Modified: " + item.lastModified + "\r\n";
        txtMyServices.Text += "Description: " + item.description + "\r\n";

        string keywords = string.Empty;
        foreach (string str in item.typeKeywords)
        {
          keywords += str + ",";
        }

        txtMyServices.Text += "Type Keywords: " + keywords + "\r\n";
        txtMyServices.Text += "ID: " + item.id + "\r\n";
        txtMyServices.Text += "Uploaded: " + item.uploaded + "\r\n";
        txtMyServices.Text += "Access: " + item.access + "\r\n";
        txtMyServices.Text += "Access Information: " + item.accessInformation + "\r\n";
        txtMyServices.Text += "Average Rating: " + item.avgRating + "\r\n";
        txtMyServices.Text += "Documentation: " + item.documentation + "\r\n";

        string tags = string.Empty;
        foreach (string str in item.tags)
          tags += str + ",";

        txtMyServices.Text += "Tags: " + tags + "\r\n";
        txtMyServices.Text += "Extent: \r\n";
        if (item.extent.Length != 0)
        {
          object[] obj1 = item.extent[0] as object[];
          object[] obj2 = item.extent[1] as object[];
          txtMyServices.Text += "\t xmax: " + obj2[0] + "\r\n";
          txtMyServices.Text += "\t ymax: " + obj2[1] + "\r\n";
          txtMyServices.Text += "\t xmin: " + obj1[0] + "\r\n";
          txtMyServices.Text += "\t ymin: " + obj1[1] + "\r\n";
          txtMyServices.Text += "\r\n";
        }
        txtMyServices.Text += "Guid: " + item.guid + "\r\n";
        txtMyServices.Text += "Item: " + item.item + "\r\n";
        txtMyServices.Text += "Item Type: " + item.itemType + "\r\n";
        txtMyServices.Text += "LicenseInfo: " + item.licenseInfo + "\r\n";
        txtMyServices.Text += "Modified: " + item.modified + "\r\n";
        txtMyServices.Text += "Name: " + item.name + "\r\n";
        txtMyServices.Text += "No# Comments: " + item.numComments + "\r\n";
        txtMyServices.Text += "Size: " + item.size + "\r\n";
        txtMyServices.Text += "Snippet:" + item.snippet + "\r\n";
        txtMyServices.Text += "Thumbnails: " + item.thumbnail + "\r\n";
        txtMyServices.Text += "URL: " + item.url + "\r\n";
      }
    }

    /// <summary>
    /// Navigate the Folder structure
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void cboFolders_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (cboFolders.Text == txtUserName.Text)
        txtMyOrgServices.Text = _myOrgServicesEndPoint;
      else
      {
        int start = cboFolders.Text.IndexOf("ID");

        int end = cboFolders.Text.Length;
        string split = cboFolders.Text.Substring(start, end - start);
        string ID = split.Replace("ID: ", "");

        split = cboFolders.Text.Substring(7, start - 8);
        txtMyOrgServices.Text = string.Format("{0}/{1}", _myOrgServicesEndPoint, ID);
      }

      btnOrganizationPersonalContent_Click(sender, e);
    }

    /// <summary>
    /// If the user double clicks the one of the DataGrid images lauch the corresponding
    /// map within a browser or Explorer online, CityEngine will open in a browser.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void dataViewImages_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      Image image = dataViewImages.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as Image;
      string webmap = image.Tag.ToString();
      string[] split = webmap.Split(',');

      int i = txtMyOrgServices.Text.IndexOf("sharing");
      string substring = txtMyOrgServices.Text.Substring(0, i);

      if (split[2].ToLower() == "web map")
      {
        //Give the User a choice of platform to open the Map Service within
        if (rbtnExplorer.Checked)
          System.Diagnostics.Process.Start(substring + "explorer/?open=" + split[0]);
        else
          System.Diagnostics.Process.Start(substring + "home/webmap/viewer.html?webmap=" + split[0]);
      }
      else //City Engine
        System.Diagnostics.Process.Start(substring + "apps/CEWebViewer/viewer.html?3dWebScene=" + split[0]);
    }

    #endregion

    #region Click Events

    /// <summary>
    /// Organization feature services, all of them (NB: for all Organization user accounts!)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Connect_Click(object sender, EventArgs e)
    {
      this.Cursor = Cursors.WaitCursor;
      string formattedRequest = string.Empty;
      string jsonResponse = string.Empty;

      listBox1.Items.Clear();

      //get the root directory of the service
      string rootURL = txtFeatureServices.Text;

      //check to see if we have an instantiated dictionary to store the attributes
      if (_featureServiceAttributesDataDictionary == null)
      {
        _featureServiceAttributesDataDictionary = new Dictionary<string, FeatureLayerAttributes>();
        _featureServiceRequestAndResponse = new Dictionary<string, string>();
      }
      else
      {
        _featureServiceAttributesDataDictionary.Clear();
        _featureServiceRequestAndResponse.Clear();
      }

      //query for ServiceLayers
      ServiceCatalog serviceCatalogDataContract = RequestAndResponseHandler.GetServiceCatalog(rootURL, _token, out formattedRequest, out jsonResponse);

      //Users on a trial period will be using services1 server, on failure resend to services server in case they have a full account.
      if (serviceCatalogDataContract.services == null)
      {
        rootURL = rootURL.Replace("services1", "services");
        serviceCatalogDataContract = RequestAndResponseHandler.GetServiceCatalog(rootURL, _token, out formattedRequest, out jsonResponse);

        if (serviceCatalogDataContract.services == null)
        {
          this.Cursor = Cursors.Default;
          return;
        }

        txtFeatureServices.Text = rootURL;
      }

      //display the call and response to the user
      ShowRequestResponseStrings(formattedRequest, jsonResponse);

      bool executedOnce = false;
      string serviceURL = string.Empty;
      string serviceRequest = string.Empty;
      string serviceResponse = string.Empty;

      foreach (Service service in serviceCatalogDataContract.services)
      {
        //I am only interested in FeatureServices THAT HAVE BEEN SHARED so only get the attributes for this kind of layer
        if (service.type == "FeatureServer")
        {
          //create the entire url string for the layer so we can make the query for attributes
          serviceURL = string.Format("{0}{1}/{2}/0/", rootURL, service.name,service.type);
           
          //Feature Layer Attributes
          FeatureLayerAttributes featLayerAttributes = RequestAndResponseHandler.GetFeatureServiceAttributes(serviceURL, _token, out serviceRequest, out serviceResponse);

          //store the request and response so that we can display it to the user when they click each feature service in the list
          _featureServiceRequestAndResponse.Add(service.name, serviceRequest + "$" + serviceResponse);

          //store the attributes
          _featureServiceAttributesDataDictionary.Add(service.name, featLayerAttributes);

          if (executedOnce == false)
          {
            _featureLayerAttributes = featLayerAttributes;

            //display the default symbol
            ShowSymbol(featLayerAttributes);

            if (featLayerAttributes == null)
            {
              this.Cursor = Cursors.Default;
              return;
            }

            //populate the Field Names
            if(featLayerAttributes.fields != null)
              PopulateFieldsList(featLayerAttributes.fields);

            //show feature service extent
            if(featLayerAttributes.extent != null)
              PopulateExtentValues(featLayerAttributes.extent);

            lblSelectedFS.Text = "FS: " + featLayerAttributes.name;
          }

          //lets get the data for the feature service
          //Todo
          string url = string.Format("http://services1.arcgis.com/{0}/arcgis/rest/services/{1}/FeatureServer?f=pjson", _organizationID, service.name);  

          _featureServiceInfo = RequestAndResponseHandler.GetDataContractInfo(url, DataContractsEnum.FeatureServiceInfo, out jsonResponse) as FeatureServiceInfo;

          //lets store the name of the featureLayer into the listbox
          ListViewItem item = new ListViewItem();
          item.Text = service.name;
          item.Tag = _featureServiceInfo;
          listBox1.Items.Add(item);

          if (executedOnce == false)
          {
            txtSupportedFunctions.Text = _featureServiceInfo.capabilities;
            executedOnce = true;
          }
        }
      }
      listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
      EnableControls(true);
      this.Cursor = Cursors.Default;
    }

    /// <summary>
    /// The user account signed in with publicly exposed feature services and map services (NB: Only publicly exposed for the 
    /// signed in user. Not all user accounts of the Organization).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOrganizationPersonalContent_Click(object sender, EventArgs e)
    {
      this.Cursor = Cursors.WaitCursor;
      string formattedRequest = string.Empty;
      string jsonResponse = string.Empty;

      cboFolders.SelectedIndexChanged -= cboFolders_SelectedIndexChanged;

      chkbxUseSelectedFS.Enabled = lblSelectedFS.Enabled = true;
      
      txtMyServices.Clear();
      //get the root directory of the service
      string rootURL = txtMyOrgServices.Text;

      //check to see if we have an instantiated dictionary to store the attributes
      if (_myOrganizationalContent == null)
        _myOrganizationalContent = new Dictionary<string, Item>();
      else
        _myOrganizationalContent.Clear();

      UserOrganizationContent myContent = RequestAndResponseHandler.UserOrgContent(rootURL, _token, out formattedRequest, out jsonResponse);

      if (myContent.currentFolder == null && myContent.items == null && myContent.username == null)
      {
        this.Cursor = Cursors.Default;
        cboFolders.SelectedIndexChanged += cboFolders_SelectedIndexChanged;
        cboItems.Items.Clear();
        return;
      }

      //display the call and response to the user
      ShowRequestResponseStrings(formattedRequest, jsonResponse);

      cboItems.Items.Clear();
      cboItems.Enabled = true;
      cboFolders.Items.Clear();
      cboFolders.Enabled = true;

      string key = "";
      List<Image> imagefiles = new List<Image>();
      
      //display the thumb nail for Webmaps and City Engine Web Scenes
      foreach (Item item in myContent.items)
      {
        if (item.type == "Web Map" || item.type == "CityEngine Web Scene")
        {
          int i = txtMyOrgServices.Text.IndexOf("users");
          string substring = txtMyOrgServices.Text.Substring(0, i);
          if (item.thumbnail != null)
          {
            substring += string.Format("items/{0}/info/{1}?token={2}", item.id, item.thumbnail, _token);

            Image thumbnail = RequestAndResponseHandler.GetThumbnail(substring);

            if (thumbnail != null)
            {
              thumbnail.Tag = string.Format("{0},{1},{2}",item.id, item.title, item.type);
              imagefiles.Add(thumbnail);
            }
          }
        }

        key = string.Format("Title: {0} ID: {1}", item.title, item.id);
        cboItems.Items.Add(key);
        _myOrganizationalContent.Add(key, item);
      }

      //load the images into the DataGrid:
      LoadImages(imagefiles);

      cboItems.SelectedIndex = 0;
      if(!cboItems.Items.Contains(txtUserName.Text))
        cboFolders.Items.Add(txtUserName.Text);

      if (myContent.folders != null)
      {
        foreach (var item in myContent.folders)
        {
          cboFolders.Items.Add(string.Format("Title: {0} ID: {1}", item.title, item.id));
        }
      }

      if(cboFolders.Items.Count > 0)
        cboFolders.SelectedIndex = 0;

      cboFolders.SelectedIndexChanged += cboFolders_SelectedIndexChanged;

      this.Cursor = Cursors.Default;
    }

    //NB: pure sample for feature creation to an existing feature service. 
    //the response is held onto so that we can use it for the update call where we simply update one field.
    //
    //Todo: Will make this dynamic going forward.
    private void Add_Feature_Click(object sender, EventArgs e)
    {
      EditFeatureService(EditType.add);
    }

    //update the field created above. 
    //
    //TODO: make dynamic
    private void btnUpdate_Click(object sender, EventArgs e)
    {
      EditFeatureService(EditType.update);
    }

    private void Delete_Feature_Click(object sender, EventArgs e)
    {
      EditFeatureService(EditType.delete);
    }
    
    private void btnQuery_Click(object sender, EventArgs e)
    {
      this.Cursor = Cursors.WaitCursor;

      lblEditingResponse.Text = "Success: ";

      //Help: http://services.arcgis.com/help/query.html  
      string formattedRequest = string.Empty;
      string jsonResponse;

      string layer = ((ListViewItem)listBox1.SelectedItem).Text;
      string myQueryRESTstring = string.Format("{0}{1}/FeatureServer/0/query?f=json", _baseApplyEditsURL, layer);
      //extent
      Extent extent = new Extent();
      extent.xmin = Convert.ToDouble(txtXmin.Text);
      extent.ymin = Convert.ToDouble(txtYmin.Text);
      extent.xmax = Convert.ToDouble(txtXmax.Text);
      extent.ymax = Convert.ToDouble(txtYmax.Text);
      string geometry = _javaScriptSerializer.Serialize(extent);

      //SQL where clause
      myQueryRESTstring += string.Format("&where={0} {1} '%{2}%'", cboFieldNamesQueries.Text, cboClause.Text.ToUpper(), txtQueryString.Text.Replace("'", ""));
      
      //do we want to return the geometries of the records found?
      myQueryRESTstring += string.Format("&returnGeometry={0}", chkboxReturnGeometry.Checked.ToString().ToLower());
      
      //Spatial relationship
      myQueryRESTstring += string.Format("&spatialRel=esriSpatialRel{0}", cboSpatialRelationship.Text);

      //extent
      if (cboGeometryType.Text != string.Empty)
      {
        try
        {
          myQueryRESTstring += "&geometry={\"xmin\":" + txtXmin.Text + ",\"ymin\":" + txtYmin.Text + ",\"xmax\":" + txtXmax.Text + ",\"ymax\":" + txtYmax.Text; //NB: ONLY DOING EXTENT IN THIS LOGIC
        }
        catch { }

        myQueryRESTstring += ",\"spatialReference\":{\"wkid\":102100, \"latestWkid\": 3857}}";

        //geometry type 
        myQueryRESTstring += string.Format("&geometryType=esriGeometry{0}", cboGeometryType.Text);

        //Spatial reference 
        myQueryRESTstring += string.Format("&inSR={0}", _featureLayerAttributes.extent.spatialReference.wkid);
      }

      myQueryRESTstring += "&outFields=*";

      //what is your desired spatial reference of the output geometry? NB: For now simply stating the same as the input
      myQueryRESTstring += string.Format("&outSR={0}&token={1}", _featureLayerAttributes.extent.spatialReference.wkid, _token);

      formattedRequest = myQueryRESTstring; //this is a GET call

      FeatureQueryResponse obj = RequestAndResponseHandler.FeatureQueryRequest(myQueryRESTstring, out jsonResponse);
      if (obj == null)
      {
        this.Cursor = Cursors.Default;
        return;
      }

      ShowRequestResponseStrings(formattedRequest, jsonResponse);

      Dictionary<string, object> dict;
      _dataTable.Rows.Clear();
      _dataTable.Columns.Clear();
      if (obj.features != null)
      {
        bool addColumnNamesOnce = true;
        lblNumRecs.Text = string.Format("No. Records:{0}", obj.features.Length);
        foreach (var item in obj.features)
        {
          dict = item as Dictionary<string, object>;

          Dictionary<string, object> dict2;
          object object2;
          dict.TryGetValue("attributes", out object2);
          dict2 = object2 as Dictionary<string, object>;

          if(addColumnNamesOnce)
          {
            foreach (var item2 in dict2)
              if(!_dataTable.Columns.Contains(item2.Key.ToString()))
                _dataTable.Columns.Add(item2.Key.ToString(), item2.Value == null ? typeof(string) : item2.Value.GetType());

            addColumnNamesOnce = false;
          }

          DataRow row = _dataTable.NewRow();

          foreach (var item2 in dict2)
          {
            row[item2.Key.ToString()] = item2.Value;
          }

          _dataTable.Rows.Add(row);
        }
      }
      else
        lblNumRecs.Text = "No. Records: 0";


      this.Cursor = Cursors.Default;
    }

    /// <summary>
    /// Authenticate against ArcGIS.com
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAuthenticate_Click(object sender, EventArgs e)
    {
      this.Cursor = Cursors.WaitCursor;
      string formattedRequest = string.Empty;
      string jsonResponse = string.Empty;

      Authentication authenticationDataContract = RequestAndResponseHandler.AuthorizeAgainstArcGISOnline(txtUserName.Text, txtPassword.Text, txtOrgURL.Text, out formattedRequest, out jsonResponse);

      //On success store the token for further use
      if (authenticationDataContract != null)
      {
        if (authenticationDataContract.token != null)
        {
          _token = authenticationDataContract.token;

          btnSelf.Enabled = true;
        }
      }

      ShowRequestResponseStrings(formattedRequest,jsonResponse);

      this.Cursor = Cursors.Default;
    }

    private void btnCreateFeatureService_Click(object sender, EventArgs e)
    {
      this.Cursor = Cursors.WaitCursor;

      string formattedRequest = string.Empty;
      string jsonResponse = string.Empty;

      string orgEndPoint = txtOrgURL.Text;
      if (!orgEndPoint.EndsWith("/"))
        orgEndPoint += "/";

      string serviceURL = string.Format("{0}sharing/content/users/{1}/createService", orgEndPoint, txtUserName.Text);

      _featureServiceCreationResponse = RequestAndResponseHandler.CreateNewFeatureService(txtFeatureServiceName.Text, serviceURL, _token, txtOrgURL.Text + "home/content.html", out formattedRequest, out jsonResponse); 
      if (_featureServiceCreationResponse == null)
        return;

      string data = string.Format("{0}sharing/content/items/{1}/data?f=json&token={2}", txtOrgURL.Text, _featureServiceCreationResponse.ItemId, _token);
            HttpWebResponse httpResponse = RequestAndResponseHandler.HttpWebGetRequest(data, "");
            //get the JSON representation from the response
            string json = RequestAndResponseHandler.DeserializeResponse(httpResponse.GetResponseStream());
            //endtest

      try
      {
        btnAddDefinitionToLayer.Enabled = true;
        //rdbtnEveryone.Enabled = rdbtnGroup.Enabled = rdbtnOrg.Enabled = true;
        //label12.Enabled = label16.Enabled = true;
        btnCreateFeatureService.Enabled = false;
      }
      catch(Exception ex) 
      {
        Console.WriteLine(ex.Message);
      }

      ShowRequestResponseStrings(formattedRequest, jsonResponse);

      this.Cursor = Cursors.Default;
    }

    private void btnAdminRoot_Click(object sender, EventArgs e)
    {
      this.Cursor = Cursors.WaitCursor;

      string formattedRequest = string.Empty;
      string jsonResponse = string.Empty;

      int index = _baseApplyEditsURL.IndexOf("rest");
      string url = _baseApplyEditsURL.Substring(0, index);
      url += "admin?f=pjson";
      Administration administration = null;

      try
      {
        administration = RequestAndResponseHandler.GetDataContractInfo(url, DataContractsEnum.Administration, out jsonResponse) as Administration;
      }
      catch { }

      //On failure change from the trial server (Services1) to the release server endpoint.
      if (administration.currentVersion == null)
      {
        formattedRequest = url = url.Replace("services1", "services");

        administration = RequestAndResponseHandler.GetDataContractInfo(url, DataContractsEnum.Administration, out jsonResponse) as Administration;

        if (administration != null)
        {
          _baseApplyEditsURL.Replace("services1", "services");
          txtFeatureServices.Text = txtFeatureServices.Text.Replace("services1", "services");
        }
      }

      //Display request and response JSON
      ShowRequestResponseStrings(formattedRequest, jsonResponse);

      lblCurrentVersion.Text = administration.currentVersion;
      lblResources.Text = string.Empty;
      lblServerType.Text = string.Empty;

      if (administration.resources == null)
      {
        this.Cursor = Cursors.Default;
        return;
      }

      int length = administration.resources.Length;
      int count = 0;
      foreach (var item in administration.resources)
      {
        if (count != (length - 1))
          lblResources.Text += item + ", ";
        else
          lblResources.Text += item;

        count++;
      }

      lblServerType.Text = administration.serverType;
      this.Cursor = Cursors.Default;
    }

    /// <summary>
    /// Before we can create a feature service within the User's organizational account, it is prudent to check that the 
    /// name is available for use, otherwise creation will fail.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnIsFeatureServiceNameAvailable_Click(object sender, EventArgs e)
    {
      this.Cursor = Cursors.WaitCursor;

      string formattedRequest = string.Empty;
      string jsonResponse = string.Empty;

      if (string.IsNullOrEmpty(txtFeatureServiceName.Text)) 
        return;

      string organizationEndpoint = txtOrgURL.Text;
      if (!organizationEndpoint.EndsWith("/"))
        organizationEndpoint += "/";

      organizationEndpoint += string.Format("sharing/portals/{0}", _organizationID);
      bool isAvailable = RequestAndResponseHandler.IsFeatureServiceNameAvailable(txtFeatureServiceName.Text, organizationEndpoint, _token, txtOrgURL.Text, out formattedRequest, out jsonResponse);
      if (isAvailable)
      {
        btnCreateFeatureService.Enabled = true;
      }
      else
      {
        btnCreateFeatureService.Enabled = false;
        btnAddDefinitionToLayer.Enabled = false;
      }

      ShowRequestResponseStrings(formattedRequest, jsonResponse);

      this.Cursor = Cursors.Default;
    }

    /// <summary>
    /// NB: This code creates a point feature service if nothing has been selected in the "My Feature Services Info -> Items combobox
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddDefinitionToLayer_Click(object sender, EventArgs e)
    {
      Item item = null;
      Extent extent = null;
      Symbol symbol = null;
      Renderer renderer = null;
      DrawingInfo drawingInfo = null;
      object[] fields = null;
      Template template = null;
      EditorTrackingInfo editorTrackingInfo = null;
      AdminLayerInfoAttribute adminLayerInfo = null;
      DefinitionLayer layer = null;
      string formattedRequest = string.Empty;
      string jsonResponse = string.Empty;

      FeatureLayerAttributes featLayerAttributes = null;

      this.Cursor = Cursors.WaitCursor;

      //NB: From the Items combobox if you have a feature service selected, this is the attribute table structure that will be used for the 
      //creation of the new service. Otherwise create a new featureLayerAttributes class as set up in this code.
      //
      if (chkbxUseSelectedFS.Checked) 
      {
        string[] concatenatedText = cboItems.Text.Split(':');
        string split = concatenatedText[1].Replace(" ID", "");
        if (_myOrganizationalContent.TryGetValue(cboItems.Text, out item))
          if(item != null)
            if(item.url != null)
              featLayerAttributes = RequestAndResponseHandler.GetFeatureServiceAttributes(item.url, _token, out formattedRequest, out jsonResponse);

        try
        {
          if (featLayerAttributes == null)
            _featureServiceAttributesDataDictionary.TryGetValue(split.Trim(), out _featureLayerAttributes);
        }
        catch { }
      }

      if (featLayerAttributes == null)
        if (_featureLayerAttributes != null)
          featLayerAttributes = _featureLayerAttributes;
        else
          featLayerAttributes = new FeatureLayerAttributes();

      //ensure that we have all that we need for a successful feature layer attributes push
      //
      if(featLayerAttributes.extent != null)
        extent = featLayerAttributes.extent; 
      else
      {
        //write in your default extent values here:
        extent = new Extent()
        {
          xmin = -14999999.999999743,
          ymin = 1859754.5323447795,
          xmax = -6199999.999999896,
          ymax = 7841397.327701188,
          spatialReference = new SpatialReference() { wkid = 102100,  latestWkid = 3857 },
        };
      }

      if (featLayerAttributes.drawingInfo != null)
        drawingInfo = featLayerAttributes.drawingInfo;
      else
      {
        symbol = new PointSymbol()
        {
          type = "esriPMS",
          url = "RedSphere.png",
          imageData = "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",
          contentType = "image/png",
          color = null,
          width = 15,
          height = 15,
          angle = 0,
          xoffset = 0,
          yoffset = 0
        };

        renderer = new PointRenderer()
        {
          type = "simple",
          symbol = symbol,
          label = "",
          description = ""
        };

        drawingInfo = new DrawingInfo()
        {
          renderer = renderer,
          labelingInfo = null
        };
      }

      if (featLayerAttributes.fields != null)
        fields = featLayerAttributes.fields;
      else
      {
        Field field = new Field()
        {
          name = "Longitude",
          type = "esriFieldTypeDouble",
          alias = "Longitude",
          sqlType = "sqlTypeFloat",
          nullable = true,
          editable = true,
          domain = null,
          defaultValue = null
        };

        Field field2 = new Field()
        {
          name = "Latitude",
          type = "esriFieldTypeDouble",
          alias = "Latitude",
          sqlType = "sqlTypeFloat",
          nullable = true,
          editable = true,
          domain = null,
          defaultValue = null
        };

        FieldString field3 = new FieldString()
        {
          name = "Name",
          type = "esriFieldTypeString",
          alias = "Name",
          sqlType = "sqlTypeNVarchar",
          length = 256,
          nullable = true,
          editable = true,
          domain = null,
          defaultValue = null
        };

        FieldString field4 = new FieldString()
        {
          name = "Address",
          type = "esriFieldTypeString",
          alias = "Address",
          sqlType = "sqlTypeNVarchar",
          length = 256,
          nullable = true,
          editable = true,
          domain = null,
          defaultValue = null
        };

        //DO NOT CHANGE PROPERTIES BELOW
        Field fieldFID = new Field()
        {
          name = "FID",
          type = "esriFieldTypeInteger",
          alias = "FID",
          sqlType = "sqlTypeInteger",
          nullable = false,
          editable = false,
          domain = null,
          defaultValue = null
        };

        //object array so that we can contain different types within.
        //Field type double for example does not contain a length parameter. Hence we need different field type 
        //representation. Unexpected properties for data types will cause a failure on the server end.
        fields = new object[5] { field, field2, field3, field4, fieldFID };
      }

      if (featLayerAttributes.templates != null)
       template = featLayerAttributes.templates[0];
      else
      {
        template = new Template()
        {
          name = "New Feature",
          description = "",
          drawingTool = "esriFeatureEditToolPoint",
          prototype = new Prototype()
          {
            attributes = new Attributes()
          }
        };
      }

      editorTrackingInfo = new EditorTrackingInfo()
      {
        enableEditorTracking = false,
        enableOwnershipAccessControl = false,
        allowOthersToUpdate = true,
        allowOthersToDelete = true
      };

      adminLayerInfo = new AdminLayerInfoAttribute()
      {
        geometryField = new GeometryField()
        {
          name = "Shape",
          srid = 102100
        }
      };

      layer = new DefinitionLayer()
      {
        currentVersion  = featLayerAttributes != null ? featLayerAttributes.currentVersion : 10.11,
        id = 0,
        name = featLayerAttributes != null ? featLayerAttributes.name != null ? featLayerAttributes.name : txtFeatureServiceName.Text : txtFeatureServiceName.Text,
        type = featLayerAttributes != null ? featLayerAttributes.type != null ? featLayerAttributes.type : "Feature Layer" : "Feature Layer",
        displayField  = featLayerAttributes != null ? featLayerAttributes.displayField != null ? featLayerAttributes.displayField : "" : "",
        description = "",
        copyrightText  = featLayerAttributes != null ? featLayerAttributes.copyrightText != null ? featLayerAttributes.copyrightText : "" : "",
        defaultVisibility  = featLayerAttributes != null ? featLayerAttributes.defaultVisibility != null ? featLayerAttributes.defaultVisibility : true : true,
        relationships  = featLayerAttributes != null ? featLayerAttributes.relationShips != null ? featLayerAttributes.relationShips : new object[]{} : new object[] { },
        isDataVersioned  = featLayerAttributes != null ? featLayerAttributes.isDataVersioned : false,
        supportsRollbackOnFailureParameter = true,
        supportsAdvancedQueries = true,
        geometryType = featLayerAttributes != null ? featLayerAttributes.geometryType != null ? featLayerAttributes.geometryType : "esriGeometryPoint" : "esriGeometryPoint",
        minScale = featLayerAttributes != null ? featLayerAttributes.minScale : 0,
        maxScale  = featLayerAttributes != null ? featLayerAttributes.maxScale : 0,
        extent = extent,
        drawingInfo = _javaScriptSerializer.Serialize(drawingInfo),
        allowGeometryUpdates  = featLayerAttributes != null ? featLayerAttributes.allowGeometryUpdates != null ? featLayerAttributes.allowGeometryUpdates : true : true,
        hasAttachments  = featLayerAttributes != null ? featLayerAttributes.hasAttachments : false,
        htmlPopupType  = featLayerAttributes != null ? featLayerAttributes.htmlPopupType != null ? featLayerAttributes.htmlPopupType : "esriServerHTMLPopupTypeNone" : "esriServerHTMLPopupTypeNone",
        hasM  = featLayerAttributes != null ? featLayerAttributes.hasM : false,
        hasZ  = featLayerAttributes != null ? featLayerAttributes.hasZ : false,
        objectIdField  = featLayerAttributes != null ? featLayerAttributes.objectIdField != null ? featLayerAttributes.objectIdField : "FID" : "FID",
        globalIdField  = featLayerAttributes != null ? featLayerAttributes.globalIdField != null ? featLayerAttributes.globalIdField : "" : "",
        typeIdField = featLayerAttributes != null ? featLayerAttributes.typeIdField != null ? featLayerAttributes.typeIdField : "" : "",
        fields = fields,
        types = featLayerAttributes != null ? featLayerAttributes.types != null ? featLayerAttributes.types : new object[0] : new object[0],
        templates = new Template[1] { template },
        supportedQueryFormats  = featLayerAttributes != null ? featLayerAttributes.supportedQueryFormats != null ? featLayerAttributes.supportedQueryFormats: "JSON" : "JSON",
        hasStaticData  = featLayerAttributes != null ? featLayerAttributes.hasStaticData : false,
        maxRecordCount  = 2000,//featLayerAttributes != null ? featLayerAttributes.maxRecordCount != null ? featLayerAttributes.maxRecordCount : 200000 : 200000,
        capabilities = featLayerAttributes != null ? featLayerAttributes.capabilities != null ? featLayerAttributes.capabilities : "Query,Editing,Create,Update,Delete" : "Query,Editing,Create,Update,Delete",
        //editorTrackingInfo = editorTrackingInfo,
        adminLayerInfo = adminLayerInfo
      };

      DefinitionLayer[] layers = new DefinitionLayer[1] { layer };

      AddDefinition definition = new AddDefinition()
      {
        layers = layers
      };

      string serviceEndPoint = "http://services1.arcgis.com/"; //NB: Trial Account endpoint!!!!
      string serviceEndPoint2 = "http://services.arcgis.com/";

      string requestURL = string.Format("{0}{1}/arcgis/admin/services/{2}.FeatureServer/AddToDefinition", serviceEndPoint, _organizationID, _featureServiceCreationResponse.Name);

      bool b = RequestAndResponseHandler.AddToFeatureServiceDefinition(requestURL, definition, _token, txtOrgURL.Text, out formattedRequest, out jsonResponse);

      if (!b)
      {
        requestURL = string.Format("{0}{1}/arcgis/admin/services/{2}.FeatureServer/AddToDefinition", serviceEndPoint2, _organizationID, _featureServiceCreationResponse.Name);
        b = RequestAndResponseHandler.AddToFeatureServiceDefinition(requestURL, definition, _token, txtOrgURL.Text, out formattedRequest, out jsonResponse);
      }

      ShowRequestResponseStrings(formattedRequest, jsonResponse);

      lblSuccess.Text = b == true ? "true" : "false";

      this.Cursor = Cursors.Default;
    }

    /// <summary>
    /// Get data on Organizational content, and that of the user specific.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSelf_Click(object sender, EventArgs e)
    {
      //self
      this.Cursor = Cursors.WaitCursor;
      txtMyOrgServices.TextChanged -= txtMyOrgServices_TextChanged;
      string formattedRequest;
      string responseJSON;

      Self response = RequestAndResponseHandler.SelfWebRequest("http://www.arcgis.com/sharing/rest/community/self", _token, out formattedRequest, out responseJSON);

      txtResponse.Text = responseJSON;
      txtRequest.Text = formattedRequest;

      _organizationID = response.orgId;
      lblFullName.Text = response.fullName;
      lblEmail.Text = response.email;
      lblStorageQuota.Text = response.storageQuota.ToString();
      lblStorageUsage.Text = response.storageUsage.ToString();
      lblOrgID.Text = response.orgId;
      lblUserOrgRole.Text = response.role;

      //Organizational Publicly exposed layers
      txtFeatureServices.Text = string.Format("http://services1.arcgis.com/{0}/ArcGIS/rest/services/", response.orgId);
      
      //user only content.
      txtMyOrgServices.Text = _myOrgServicesEndPoint = string.Format("{0}sharing/content/users/{1}", txtOrgURL.Text, txtUserName.Text);

      cboFolders.Items.Clear();
      cboItems.Items.Clear();
      txtMyServices.Text = string.Empty;
      listBox1.Items.Clear();
      cboFieldNames.Items.Clear();
      dataViewImages.Rows.Clear();
      dataViewImages.Columns.Clear();

      btnIsNameAvailable.Enabled = btnAdminRoot.Enabled = true;
      label12.Enabled = txtFeatureServiceName.Enabled = true;
      txtMyOrgServices.TextChanged += txtMyOrgServices_TextChanged;
      txtMyOrgServices_TextChanged(sender, e);
      this.Cursor = Cursors.Default;
    }

    #endregion
  }
}
