namespace AGORestCallTestFS
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
      UserOrganizationContent
    };

    public enum AddItemType
    {
      csv,
      gpx,
      shapefile
    }

    public enum ESRIGeometryTypes
    {
      esriGeometryPoint,
      esriGeometryMultipoint,
      esriGeometryPolyline,
      esriGeometryPolygon,
      esriGeometryEnvelope
    }

    public enum ESRISpatialRelationship
    {
      esriSpatialRelIntersects,
      esriSpatialRelContains,
      esriSpatialRelCrosses,
      esriSpatialRelOverlaps,
      esriSpatialRelTouches,
      esriSpatialRelWithin
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
