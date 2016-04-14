using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ODataRestierDynamic.DynamicFactory
{
	public enum DBObjectType
	{
		Table,
		View,
		Procedure,
		Function,
	}

	public class DynamicMetadataObject
	{
		public string Name { get; set; }

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

		public string Schema { get; set; }

		[NotMapped]
		public DBObjectType ObjectType { get; set; }
	}
}