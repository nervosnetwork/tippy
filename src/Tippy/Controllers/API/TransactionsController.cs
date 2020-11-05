using Microsoft.AspNetCore.Mvc;
using Tippy.MockApiData;

namespace Tippy.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        [HttpGet]
        public ActionResult Index()
        {
            return Ok(Loader.JsonFromFile("Transactions"));
        }
    }
}
