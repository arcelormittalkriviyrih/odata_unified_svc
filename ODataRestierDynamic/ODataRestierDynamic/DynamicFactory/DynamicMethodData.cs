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
		public DynamicParameterData[] Params { get; set; }

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

	/// <summary>	A dynamic parameter data. For generating by DynamicClassFactory. </summary>
	public class DynamicParameterData
	{
		/// <summary>	Gets or sets the type of the parameter. </summary>
		///
		/// <value>	The parameter Type. </value>
		public Type Type { get; set; }

		/// <summary>	Gets or sets a name of the parameter. </summary>
		///
		/// <value>	A name of the parameter. </value>
		public string Name { get; set; }

		/// <summary>	Gets or sets an input type of parameter. </summary>
		///
		/// <value>	Is parameter input. </value>
		public bool isIn { get; set; }

		/// <summary>	Gets or sets an output type of parameter. </summary>
		///
		/// <value>	Is parameter output. </value>
		public bool isOut { get; set; }

		/// <summary>	Gets or sets the length. </summary>
		///
		/// <value>	The length. </value>
		public int? Length { get; set; }
	}
}