using Microsoft.Restier.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataRestierDynamic.DynamicFactory
{
	/// <summary>
	/// Support hook handler chain, could choose to implement the following interface besides the IHookHandler.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IDelegateHookHandler<T> where T : IHookHandler
	{
		T InnerHandler { get; set; }
	}
}