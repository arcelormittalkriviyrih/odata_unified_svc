using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataRestierDynamic.CustomAttributes
{
	/// <summary>	Attribute for sequence. This class cannot be inherited. </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class SequenceAttribute : Attribute
	{
		/// <summary>	Gets or sets the script. </summary>
		///
		/// <value>	The script. </value>
		public string Script { get; private set; }

		/// <summary>	Constructor. </summary>
		///
		/// <param name="script">	The script. </param>
		public SequenceAttribute(string script)
		{
			this.Script = script;
		}
	}
}