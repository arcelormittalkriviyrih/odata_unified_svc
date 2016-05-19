using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace ODataRestierDynamic.Actions
{
	/// <summary>	Encapsulates the result of an action. </summary>
	[XmlRoot(ElementName = "ActionResult")]
	public class ActionResult
	{
		/// <summary>	Gets or sets the name. </summary>
		///
		/// <value>	The name. </value>
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }

		/// <summary>	Gets or sets options for controlling the action. </summary>
		///
		/// <value>	Options that control the action. </value>
		[XmlElement(ElementName = "Parameter")]
		public List<ActionParameter> ActionParameters { get; set; }

		/// <summary>	Gets or sets the return. </summary>
		///
		/// <value>	The return. </value>
		[XmlElement(ElementName = "Return")]
		public object Return { get; set; }
	}

	/// <summary>	An action parameter. </summary>
	[XmlRoot(ElementName = "ActionParameter")]
	public class ActionParameter
	{
		/// <summary>	Gets or sets the name. </summary>
		///
		/// <value>	The name. </value>
		[XmlAttribute(AttributeName = "Name")]
		public string Name { get; set; }

		/// <summary>	Gets or sets the value. </summary>
		///
		/// <value>	The value. </value>
		[XmlText]
		public object Value { get; set; }
	}
}