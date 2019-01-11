using ODataRestierDynamic.DynamicFactory;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ODataRestierDynamic.Helpers
{
    /// <summary>	A table build helper. </summary>
    public class TableBuildHelper
	{
		/// <summary>	Gets or sets the name. </summary>
		///
		/// <value>	The name. </value>
		public string Name { get; set; }

		/// <summary>	Gets or sets the type builder. </summary>
		///
		/// <value>	The type builder. </value>
		public TypeBuilder TypeBuilder { get; set; }

		/// <summary>	Gets or sets the properties. </summary>
		///
		/// <value>	The properties. </value>
		public List<TablePropertyBuildHelper> Properties { get; set; }
	}

	/// <summary>	A table property build helper. </summary>
	public class TablePropertyBuildHelper
	{
		/// <summary>	Gets or sets the name. </summary>
		///
		/// <value>	The name. </value>
		public string Name { get; set; }

		/// <summary>	Gets or sets the data. </summary>
		///
		/// <value>	The data. </value>
		public DynamicPropertyData Data { get; set; }
	}

	/// <summary>	A table helper extensions. </summary>
	public static class TableHelperExtensions
	{
		/// <summary>
		/// A List&lt;TableBuildHelper&gt; extension method that searches for the first match.
		/// </summary>
		///
		/// <param name="tableBuildHelperList">	The tableBuildHelperList to act on. </param>
		/// <param name="tableName">		   	Name of the table. </param>
		///
		/// <returns>	A TableBuildHelper. </returns>
		public static TableBuildHelper Find(this List<TableBuildHelper> tableBuildHelperList, string tableName)
		{
			return tableBuildHelperList.Find(t => t.Name == tableName);
		}

		/// <summary>
		/// A List&lt;TablePropertyBuildHelper&gt; extension method that searches for the first match.
		/// </summary>
		///
		/// <param name="properties">	The properties to act on. </param>
		/// <param name="columnName">	Name of the column. </param>
		///
		/// <returns>	A TablePropertyBuildHelper. </returns>
		public static TablePropertyBuildHelper Find(this List<TablePropertyBuildHelper> properties, string columnName)
		{
			return properties.Find(t => t.Data.ColumnName == columnName);
		}

		/// <summary>
		/// A List&lt;TablePropertyBuildHelper&gt; extension method that determine if 'properties'
		/// exists.
		/// </summary>
		///
		/// <param name="properties">  	The properties to act on. </param>
		/// <param name="propertyName">	Name of the property. </param>
		///
		/// <returns>	true if it succeeds, false if it fails. </returns>
		public static bool Exists(this List<TablePropertyBuildHelper> properties, string propertyName)
		{
			return properties.Exists(p => p.Name == propertyName);
		}
	}
}