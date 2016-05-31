using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODataRestierDynamic.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ODataRestierDynamic.Models.Tests
{
	[TestClass()]
	public class DynamicContextTests
	{
		[TestMethod()]
		public void DynamicContextTest()
		{
			var dynamicContext = new DynamicContext();

			//Check Tables
			Assert.IsNotNull(dynamicContext.GetModelType("TestParent"));
			Assert.IsNotNull(dynamicContext.GetModelType("TestChild"));

			//Check Views
			Assert.IsNotNull(dynamicContext.GetModelType("v_TestView"));

			//Check Actions
			Assert.IsTrue(dynamicContext.DynamicActionMethods.ContainsKey("ins_TestParent"));
		}
	}
}
