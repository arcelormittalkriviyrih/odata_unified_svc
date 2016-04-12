using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataRestierDynamic.DynamicFactory
{
	public class DynamicPropertyData
	{
		public bool IsPrimaryKey { get; set; }

		public bool IsForeignKey { get; set; }

		public int Order { get; set; }

		public bool Nullable { get; set; }

		public int? MaxLength { get; set; }

		public Type Type { get; set; }
	}
}