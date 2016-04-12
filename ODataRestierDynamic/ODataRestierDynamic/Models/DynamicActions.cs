using EntityFramework.Functions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using Microsoft.Restier.Core.Model;
using System.Data.Entity;


namespace ODataRestierDynamic.Models
{
	public class DynamicActions
	{
		private DbContext _dbContext = null;

		public DynamicActions(DbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[Action]
		[EntityFramework.Functions.Function(FunctionType.StoredProcedure, "ins_MaterialLotByController", Schema = DynamicContext.cDefaultSchemaName)]
		public int ins_MaterialLotByController(Nullable<int> controllerID)
		{
			var controllerIDParameter = controllerID.HasValue ?
				new ObjectParameter("ControllerID", controllerID) :
				new ObjectParameter("ControllerID", typeof(int));

			return ((IObjectContextAdapter)_dbContext).ObjectContext.ExecuteFunction("ins_MaterialLotByController", controllerIDParameter);
		}
	}
}