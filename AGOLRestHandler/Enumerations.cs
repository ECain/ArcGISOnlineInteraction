/*
 * Author: Edan Cain, ESRI, 380 New York Street, Redlands, California, 92373, USA. Email: ecain@esri.com Tel: 1-909-793-2853
 * 
 * Code demonstrates enumerations required for REST calls for interaction with ArcGIS.com organization accounts.
 * 
 * Code is not supported by ESRI inc, there are no use restrictions, you are free to distribute, modify and use this code.
 * Enhancement or functional code requests should be sent to Edan Cain, ecain@esri.com. 
 * 
 * Code only supports Feature Services / Map Services.
 * 
 * Code created to help support the Start-up community by the ESRI Emerging Business Team. If you are a start up company,
 * please contact Myles Sutherland at msutherland@esri.com.
 */

namespace AGOLRestHandler
{
    public enum DataContractsEnum
    {
      Administration,
      ServiceCatalog,
      FeatureLayerAttributes,
      FeatureEditsResponse,
      FeatureQueryResponse,
      FeatureService,
      FeatureServiceInfo,
      FeatureServiceNameAvailability,
      UserOrganizationContent,
      GeometryResponse
    };

    public enum ESRIGeometryTypes
    {
      esriGeometryPoint,
      esriGeometryMultipoint,
      esriGeometryPolyline,
      esriGeometryPolygon,
      esriGeometryEnvelope
    }

    /// <summary>
    /// http://edndoc.esri.com/arcobjects/8.3/componenthelp/esriCore/esriSpatialRelEnum.htm
    /// </summary>
    public enum ESRISpatialRelationship
    {
      esriSpatialRelUndefined,
      esriSpatialRelIntersects,
      esriSpatialRelEnvelopeIntersects,
      esriSpatialRelIndexIntersects,
      esriSpatialRelTouches,
      esriSpatialRelOverlaps,
      esriSpatialRelCrosses,
      esriSpatialRelWithin,
      esriSpatialRelContains,
      esriSpatialRelRelation
    }

    public enum PublishItemType
    {
      csv,
      shapefile,
      serviceDefinition
    }

    public enum EsriFieldType
    {
      esriFieldTypeSmallInteger,
      esriFieldTypeInteger,
      esriFieldTypeSingle,
      esriFieldTypeDouble,
      esriFieldTypeString,
      esriFieldTypeDate,
      esriFieldTypeOID,
      esriFieldTypeGeometry,
      esriFieldTypeBlob,
      esriFieldTypeRaster,
      esriFieldTypeGUID,
      esriFieldTypeGlobalID
    }
  
}
