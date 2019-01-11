using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ODataRestierDynamic.Models
{
    /// <summary>	Values that represent database object types. </summary>
    public enum DBObjectType
	{
		/// <summary>	An enum constant representing the table option. </summary>
		Table,
		/// <summary>	An enum constant representing the view option. </summary>
		View,
		/// <summary>	An enum constant representing the procedure option. </summary>
		Procedure,
		/// <summary>	An enum constant representing the function option. </summary>
		Function,
	}

	/// <summary>	A dynamic metadata object. </summary>
	public class DynamicMetadataObject
	{
		/// <summary>	Gets or sets the name. </summary>
		///
		/// <value>	The name. </value>
		public string Name { get; set; }

		/// <summary>	Gets or sets the type. </summary>
		///
		/// <value>	The type. </value>
		public string Type
		{
			get
			{
				return ObjectType.ToString();
			}
			set
			{
				this.ObjectType = (DBObjectType)Enum.Parse(typeof(DBObjectType), value);
			}
		}

		/// <summary>	Gets or sets the schema. </summary>
		///
		/// <value>	The schema. </value>
		public string Schema { get; set; }

		/// <summary>	Gets or sets the type of the object. </summary>
		///
		/// <value>	The type of the object. Table View etc. Not mapped to context (will not be visible in OData object). </value>
		[NotMapped]
		public DBObjectType ObjectType { get; set; }
	}
}