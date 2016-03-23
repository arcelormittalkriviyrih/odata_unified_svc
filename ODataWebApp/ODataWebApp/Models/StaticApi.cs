using Microsoft.Restier.Core.Model;
using Microsoft.Restier.EntityFramework;
using ODataWebApp.DynamicFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataWebApp.Models
{
	public class StaticApi : DbApi<Entities>
	{
		public Entities Context
		{
			get
			{
				return DbContext;
			}
		}

		[Action]
		public int ins_MaterialLotByController(int controllerID)
		{
			return -1;
		}
	}
}