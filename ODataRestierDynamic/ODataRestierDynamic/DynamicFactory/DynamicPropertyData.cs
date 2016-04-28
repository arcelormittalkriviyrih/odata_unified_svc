using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataRestierDynamic.DynamicFactory
{
	/// <summary>	Base class for dynamic property data. Needed for dynamic object factory. </summary>
	public class DynamicPropertyData
	{
		/// <summary>	Gets or sets a value indicating whether this object is primary key. </summary>
		///
		/// <value>	true if this object is primary key, false if not. </value>
		public bool IsPrimaryKey { get; set; }

		/// <summary>	Gets or sets a value indicating whether this object is foreign key. </summary>
		///
		/// <value>	true if this object is foreign key, false if not. </value>
		public bool IsForeignKey { get; set; }

		/// <summary>	Gets or sets the order. </summary>
		///
		/// <value>	The order. </value>
		public int Order { get; set; }

		/// <summary>	Gets or sets a value indicating whether this object is nullable. </summary>
		///
		/// <value>	true if nullable, false if not. </value>
		public bool Nullable { get; set; }

		/// <summary>	Gets or sets the length of the maximum. </summary>
		///
		/// <value>	The length of the maximum. </value>
		public int? MaxLength { get; set; }

		/// <summary>	Gets or sets the type. </summary>
		///
		/// <value>	The type. </value>
		public Type Type { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this object is computed identifier.
		/// </summary>
		///
		/// <value>	true if this object is computed identifier, false if not. </value>
		public bool IsComputedID { get; set; }

		/// <summary>	Gets or sets the sequence script needed for adding new entity. </summary>
		///
		/// <value>	The sequence script needed for adding new entity. </value>
		public string SequenceScript { get; set; }

		/// <summary>	Gets or sets the name of the column. </summary>
		///
		/// <value>	The name of the column. </value>
		public string ColumnName { get; set; }
	}
}