using System.Web.OData;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using ODataApi.DataSource;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using System.Web.Http.Dispatcher;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Net.Http;
using DatabaseSchemaReader.DataSchema;
using System;
using System.Web.OData.Query;
using DatabaseSchemaReader.Data;
using DatabaseSchemaReader;
using System.Linq;

namespace ODataApi.Controllers
{
	public class HandleAllController : ODataController
	{
		private ODataValidationSettings _ValidationSettings = new ODataValidationSettings();

		public HandleAllController()
		{
			_ValidationSettings.AllowedQueryOptions = AllowedQueryOptions.All;
		}

		// Get entityset
		//public EdmEntityObjectCollection Get()
		//{
		//	// Get entity set's EDM type: A collection type.
		//	ODataPath path = Request.ODataProperties().Path;
		//	IEdmCollectionType collectionType = (IEdmCollectionType)path.EdmType;
		//	IEdmEntityTypeReference entityType = collectionType.ElementType.AsEntity();

		//	// Create an untyped collection with the EDM collection type.
		//	EdmEntityObjectCollection collection =
		//		new EdmEntityObjectCollection(new EdmCollectionTypeReference(collectionType));

		//	// Add untyped objects to collection.
		//	DataSourceProvider.Get((string)Request.Properties[Constants.ODataDataSource], entityType, collection);

		//	return collection;
		//}

		// Get entityset(key)
		[EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
		public IEdmEntityObject Get(string key)
		{
			// Get entity type from path.
			ODataPath path = Request.ODataProperties().Path;
			IEdmEntityType entityType = (IEdmEntityType)path.EdmType;

			// Create an untyped entity object with the entity type.
			EdmEntityObject entity = new EdmEntityObject(entityType);

			DataSourceProvider.Get((string)Request.Properties[Constants.ODataDataSource], key, entity);

			return entity;
		}

		[EnableQuery(PageSize = 20, AllowedQueryOptions = AllowedQueryOptions.All)]
		public IHttpActionResult Get()
		{
			// Get entity set's EDM type: A collection type.
			ODataPath path = Request.ODataProperties().Path;
			IEdmCollectionType collectionType = (IEdmCollectionType)path.EdmType;
			IEdmEntityTypeReference entityType = collectionType.ElementType.AsEntity();

			// Create an untyped collection with the EDM collection type.
			EdmEntityObjectCollection collection =
				new EdmEntityObjectCollection(new EdmCollectionTypeReference(collectionType));

			// Add untyped objects to collection.
			DataSourceProvider.Get((string)Request.Properties[Constants.ODataDataSource], entityType, collection);

			return Ok(collection);
		}
	}
}
