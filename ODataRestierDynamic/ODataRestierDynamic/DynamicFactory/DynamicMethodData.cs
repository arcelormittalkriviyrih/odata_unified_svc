using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataRestierDynamic.DynamicFactory
{
	public class DynamicMethodData
	{
		public Type[] Params { get; set; }

		public string[] ParamNames { get; set; }

		public Type ReturnType { get; set; }

		public EntityFramework.Functions.FunctionType FunctionType { get; set; }

		public string Schema { get; set; }
	}
}