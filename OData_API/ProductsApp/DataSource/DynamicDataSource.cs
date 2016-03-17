using System.Web.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using System.Web.Configuration;
using ODataApi.DBConnection;
using DatabaseSchemaReader;
using DatabaseSchemaReader.DataSchema;
using System;
using DatabaseSchemaReader.Data;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http;
using System.Linq;
using ODataApi.Controllers;
using System.Web.OData.Query;

namespace ODataApi.DataSource
{
	internal class DynamicDataSource : IDataSource
	{
		private string _DataSourceName;
		private string _ConnectionString;

		private Dictionary<string, DatabaseTable> _tables = new Dictionary<string, DatabaseTable>(StringComparer.OrdinalIgnoreCase);
		private Dictionary<string, DatabaseView> _views = new Dictionary<string, DatabaseView>(StringComparer.OrdinalIgnoreCase);
		private Dictionary<string, DatabaseFunction> _functions = new Dictionary<string, DatabaseFunction>(StringComparer.OrdinalIgnoreCase);
		private Dictionary<string, DatabaseStoredProcedure> _procedures = new Dictionary<string, DatabaseStoredProcedure>(StringComparer.OrdinalIgnoreCase);

		internal DynamicDataSource(string dataSourceName)
		{
			//if (dataSourceName.ToLowerInvariant() == Constants.DefaultDataSource.ToLowerInvariant())
			//	_DataSourceName = Constants.DefaultDataSource;
			//else
			_DataSourceName = dataSourceName;

			System.Configuration.ConnectionStringSettings connectionStringSettings = WebConfigurationManager.ConnectionStrings[_DataSourceName];
			if (connectionStringSettings == null)
			{
				throw new System.InvalidOperationException(string.Format("Data source: {0} is not registered.", dataSourceName));
			}
			else
			{
				_ConnectionString = connectionStringSettings.ConnectionString;
			}
		}

		public void GetModel(EdmModel model, EdmEntityContainer container)
		{
			// https://www.nuget.org/packages/DatabaseSchemaReader/
			using (var dbReader = new DatabaseReader(_ConnectionString, "System.Data.SqlClient"))
			{
				var schema = dbReader.ReadAll();

				#region Read Tables

				foreach (var table in schema.Tables)
				{
					EdmEntityType tableType = new EdmEntityType(_DataSourceName, table.Name);
					foreach (var col in table.Columns)
					{
						var kind = GetKind(col.DataType);
						if (!kind.HasValue) //|| kind.Value == EdmPrimitiveTypeKind.DateTimeOffset) // don't map this
							continue;

						var prop = tableType.AddStructuralProperty(col.Name, kind.Value, col.Nullable);
						if (col.IsPrimaryKey)
						{
							tableType.AddKeys(prop);
						}
					}
					model.AddElement(tableType);

					EdmEntitySet anotherTable = container.AddEntitySet(table.Name, tableType);
					if (!_tables.ContainsKey(table.Name))
						_tables.Add(table.Name, table);
				}

				#endregion

				#region Read Views

				foreach (var view in schema.Views)
				{
					EdmEntityType viewType = new EdmEntityType(_DataSourceName, view.Name);
					foreach (var col in view.Columns)
					{
						var kind = GetKind(col.DataType);
						if (!kind.HasValue) //|| kind.Value == EdmPrimitiveTypeKind.DateTimeOffset) // don't map this
							continue;

						var prop = viewType.AddStructuralProperty(col.Name, kind.Value, col.Nullable);
						if (col.IsPrimaryKey)
						{
							viewType.AddKeys(prop);
						}
					}
					model.AddElement(viewType);

					EdmEntitySet anotherTable = container.AddEntitySet(view.Name, viewType);
					if (!_views.ContainsKey(view.Name))
						_views.Add(view.Name, view);
				}

				#endregion

				#region Read Functions

				foreach (var function in schema.Functions)
				{
					EdmEntityType tableType = new EdmEntityType(_DataSourceName, function.Name);
					var returnKind = GetKind(function.ReturnType);
					if (returnKind.HasValue)
						tableType.AddStructuralProperty("ReturnType", returnKind.Value);
					foreach (var arguments in function.Arguments)
					{
						var kind = GetKind(arguments.DataType);
						if (!kind.HasValue) //|| kind.Value == EdmPrimitiveTypeKind.DateTimeOffset) // don't map this
							continue;

						tableType.AddStructuralProperty(arguments.Name, kind.Value);
					}
					model.AddElement(tableType);

					EdmEntitySet anotherTable = container.AddEntitySet(function.Name, tableType);
					if (!_functions.ContainsKey(function.Name))
						_functions.Add(function.Name, function);
				}

				#endregion

				#region Read Procedures

				foreach (var procedure in schema.StoredProcedures)
				{
					EdmEntityType tableType = new EdmEntityType(_DataSourceName, procedure.Name);
					foreach (var arguments in procedure.Arguments)
					{
						var kind = GetKind(arguments.DataType);
						if (!kind.HasValue) //|| kind.Value == EdmPrimitiveTypeKind.DateTimeOffset) // don't map this
							continue;

						tableType.AddStructuralProperty(arguments.Name, kind.Value);
					}
					model.AddElement(tableType);

					EdmEntitySet anotherTable = container.AddEntitySet(procedure.Name, tableType);
					if (!_procedures.ContainsKey(procedure.Name))
						_procedures.Add(procedure.Name, procedure);
				}

				#endregion
			}
		}

		public void Get(IEdmEntityTypeReference entityType, EdmEntityObjectCollection collection)
		{
			EdmEntityType type = entityType.Definition as EdmEntityType;

			#region Tables

			DatabaseTable table = null;
			if (_tables.ContainsKey(type.Name))
				table = _tables[type.Name];

			if (table != null)
			{
				// https://www.nuget.org/packages/DatabaseSchemaReader/
				Reader reader = new Reader(table, _ConnectionString, "System.Data.SqlClient");
				reader.Read((r) =>
				{
					EdmEntityObject entity = new EdmEntityObject(type);
					foreach (var prop in type.DeclaredProperties)
					{
						int index = r.GetOrdinal(prop.Name);
						object value = r.GetValue(index);
						if (Convert.IsDBNull(value))
						{
							value = null;
						}
						else if (value is DateTime)
						{
							DateTimeOffset convertDateTime = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
							value = convertDateTime;
						}
						entity.TrySetPropertyValue(prop.Name, value);
					}

					collection.Add(entity);

					return true;
				});
			}

			#endregion

			#region Views

			DatabaseView view = null;
			if (_views.ContainsKey(type.Name))
				view = _views[type.Name];

			if (view != null)
			{
				// https://www.nuget.org/packages/DatabaseSchemaReader/
				Reader reader = new Reader(table, _ConnectionString, "System.Data.SqlClient");
				reader.Read((r) =>
				{
					EdmEntityObject entity = new EdmEntityObject(type);
					foreach (var prop in type.DeclaredProperties)
					{
						int index = r.GetOrdinal(prop.Name);
						object value = r.GetValue(index);
						if (Convert.IsDBNull(value))
						{
							value = null;
						}
						else if (value is DateTime)
						{
							DateTimeOffset convertDateTime = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
							value = convertDateTime;
						}
						entity.TrySetPropertyValue(prop.Name, value);
					}

					collection.Add(entity);

					return true;
				});
			}

			#endregion
		}

		public void Get(string key, EdmEntityObject entity)
		{
			EdmEntityType tableType = entity.ActualEdmType as EdmEntityType;
			if (tableType.DeclaredKey.Count() > 0)
			{
				var idFieldType = tableType.DeclaredKey.First();
				DbCommand dbCommandCurTable = new DbCommand();
				try
				{
					dbCommandCurTable.ConnectionString = _ConnectionString;
					dbCommandCurTable.CommandText = string.Format("select * from {0} where {1}=@key", tableType.Name, idFieldType.Name);
					if (key.StartsWith("'") && key.EndsWith("'"))
						key = key.Substring(1, key.Length - 2);
					dbCommandCurTable.AddInParameter("@key", System.Data.DbType.String, key);
					var readerCurTable = dbCommandCurTable.ExecuteReader();
					//var dataTable = new System.Data.DataTable();
					//dataTable.Load(readerCurTable);

					while (readerCurTable.Read())
					{
						foreach (var prop in tableType.DeclaredProperties)
						{
							int index = readerCurTable.GetOrdinal(prop.Name);
							object value = readerCurTable.GetValue(index);
							if (Convert.IsDBNull(value))
							{
								value = null;
							}
							else if (value is DateTime)
							{
								DateTimeOffset convertDateTime = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
								value = convertDateTime;
							}
							entity.TrySetPropertyValue(prop.Name, value);
						}
					}
				}
				finally
				{
					dbCommandCurTable.CloseConnection();
				}
			}
		}

		// determine Edm kind from column type
		private static EdmPrimitiveTypeKind? GetKind(DataType dataType)
		{
			if (dataType == null)
				return null;

			Type type = dataType.GetNetType();
			if (type == null)
				return null;

			if (type == typeof(string))
				return EdmPrimitiveTypeKind.String;

			if (type == typeof(short))
				return EdmPrimitiveTypeKind.Int16;

			if (type == typeof(int))
				return EdmPrimitiveTypeKind.Int32;

			if (type == typeof(long))
				return EdmPrimitiveTypeKind.Int64;

			if (type == typeof(bool))
				return EdmPrimitiveTypeKind.Boolean;

			if (type == typeof(Guid))
				return EdmPrimitiveTypeKind.Guid;

			if (type == typeof(DateTime))
				return EdmPrimitiveTypeKind.DateTimeOffset;

			if (type == typeof(TimeSpan))
				return EdmPrimitiveTypeKind.Duration;

			if (type == typeof(decimal))
				return EdmPrimitiveTypeKind.Decimal;

			if (type == typeof(byte) || type == typeof(sbyte))
				return EdmPrimitiveTypeKind.Byte;

			if (type == typeof(byte[]))
				return EdmPrimitiveTypeKind.Binary;

			if (type == typeof(double))
				return EdmPrimitiveTypeKind.Double;

			if (type == typeof(float))
				return EdmPrimitiveTypeKind.Single;

			return null;
		}

		// determine Edm kind from column type
		private static EdmPrimitiveTypeKind? GetKind(string type)
		{
			if (string.IsNullOrEmpty(type))
				return null;

			if (type == "string")
				return EdmPrimitiveTypeKind.String;

			if (type == "short")
				return EdmPrimitiveTypeKind.Int16;

			if (type == "int")
				return EdmPrimitiveTypeKind.Int32;

			if (type == "long")
				return EdmPrimitiveTypeKind.Int64;

			if (type == "bool")
				return EdmPrimitiveTypeKind.Boolean;

			if (type == "Guid")
				return EdmPrimitiveTypeKind.Guid;

			if (type == "DateTime")
				return EdmPrimitiveTypeKind.DateTimeOffset;

			if (type == "TimeSpan")
				return EdmPrimitiveTypeKind.Duration;

			if (type == "decimal")
				return EdmPrimitiveTypeKind.Decimal;

			if (type == "byte" || type == "sbyte")
				return EdmPrimitiveTypeKind.Byte;

			if (type == "byte[]")
				return EdmPrimitiveTypeKind.Binary;

			if (type == "double")
				return EdmPrimitiveTypeKind.Double;

			if (type == "float")
				return EdmPrimitiveTypeKind.Single;

			return null;
		}
	}
}
