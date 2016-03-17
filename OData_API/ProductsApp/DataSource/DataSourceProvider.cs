using System;
using System.Web.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using System.Collections.Generic;

namespace ODataApi.DataSource
{
	internal class DataSourceProvider
	{
		private static Dictionary<string, DynamicDataSource> _dynamicDataSource = new Dictionary<string, DynamicDataSource>(StringComparer.OrdinalIgnoreCase);

		public static IEdmModel GetEdmModel(string dataSourceName)
		{
			EdmModel model = new EdmModel();
			EdmEntityContainer container = new EdmEntityContainer(dataSourceName, "container");
			model.AddElement(container);

			GetDataSource(dataSourceName).GetModel(model, container);

			return model;
		}

		public static void Get(
			string dataSourceName,
			IEdmEntityTypeReference entityType,
			EdmEntityObjectCollection collection)
		{
			GetDataSource(dataSourceName).Get(entityType, collection);
		}

		public static void Get(string dataSourceName, string key, EdmEntityObject entity)
		{
			GetDataSource(dataSourceName).Get(key, entity);
		}

		private static IDataSource GetDataSource(string dataSourceName)
		{
			dataSourceName = dataSourceName == null ? string.Empty : dataSourceName.ToLowerInvariant();

			DynamicDataSource dataSource;

			if (_dynamicDataSource.ContainsKey(dataSourceName))
				dataSource = _dynamicDataSource[dataSourceName];
			else
			{
				dataSource = new DynamicDataSource(dataSourceName);
				_dynamicDataSource.Add(dataSourceName, dataSource);
			}

			return dataSource; //new DynamicDataSource(dataSourceName);

			//switch (dataSourceName)
			//{
			//	case Constants.MyDataSource:
			//		return new MyDataSource();
			//	case Constants.AnotherDataSource:
			//		return new AnotherDataSource();
			//	default:
			//		throw new InvalidOperationException(
			//			string.Format("Data source: {0} is not registered.", dataSourceName));
			//}
		}
	}
}
