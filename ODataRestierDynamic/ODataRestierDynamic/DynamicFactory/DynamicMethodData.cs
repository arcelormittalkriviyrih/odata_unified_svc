using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataRestierDynamic.DynamicFactory
{
	/// <summary>	A dynamic method data. For generating by DynamicClassFactory. </summary>
	public class DynamicMethodData
	{
		/// <summary>	Gets or sets options for controlling the operation. </summary>
		///
		/// <value>	The parameters. </value>
		public Type[] Params { get; set; }

		/// <summary>	Gets or sets a list of names of the parameters. </summary>
		///
		/// <value>	A list of names of the parameters. </value>
		public string[] ParamNames { get; set; }

		/// <summary>	Gets or sets the type of the return. </summary>
		///
		/// <value>	The type of the return. </value>
		public Type ReturnType { get; set; }

		/// <summary>	Gets or sets the type of the function. </summary>
		///
		/// <value>	The type of the function. </value>
		public EntityFramework.Functions.FunctionType FunctionType { get; set; }

		/// <summary>	Gets or sets the schema. </summary>
		///
		/// <value>	The schema. </value>
		public string Schema { get; set; }
	}
}