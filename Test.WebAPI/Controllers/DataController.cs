using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Test.WebAPI.Controllers
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class DataController : ODataController
	{
		private TestDataContext testDataContext { get; set; }

		public IConfiguration Configuration { get; set; }
		public ILogger _logger;

		public DataController(TestDataContext testDataContext, IConfiguration configuration, ILogger<DataController> logger)
		{
			this.testDataContext = testDataContext;
			Configuration = configuration;
			_logger = logger;
		}


		[HttpGet("{entity}")]
		[EnableQueryWithMetadata]
		public IActionResult Get(string entity)
		{

			_logger.LogInformation($"Getting {entity}");
			if (entity.Equals("users", StringComparison.InvariantCultureIgnoreCase))
				return Ok(this.testDataContext.Users);
			else
			{
				_logger.LogInformation($"Getting {entity} error. Not Found.");
				return new NotFoundResult();
			}
		}
	}
}
