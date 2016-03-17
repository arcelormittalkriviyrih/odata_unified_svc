using Microsoft.Restier.EntityFramework;
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
	}
}