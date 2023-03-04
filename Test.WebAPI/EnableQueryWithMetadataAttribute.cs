using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.OData.Extensions;

namespace Test.WebAPI
{
	public class EnableQueryWithMetadataAttribute : EnableQueryAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
		{
			try
			{
				base.OnActionExecuted(actionExecutedContext);
				var odata = actionExecutedContext.HttpContext.ODataFeature();
				if (odata.TotalCount.HasValue)
					actionExecutedContext.HttpContext.Response.Headers.Add("x-total-count", odata.TotalCount.Value.ToString("#0"));

				//if (actionExecutedContext.Result is ObjectResult obj && obj.Value is IQueryable qry)
				//{
				//	obj.Value = new ODataResponse
				//	{
				//		Count = actionExecutedContext.HttpContext.Request.ODataFeature().TotalCount,
				//		Value = qry
				//	};
				//}
			}
			catch (System.Exception)
			{
				//throw;
			}
		}

		public class ODataResponse
		{
			[JsonPropertyName("@odata.count")]
			public long? Count { get; set; }

			[JsonPropertyName("value")]
			public IQueryable Value { get; set; }
		}
	}
}
