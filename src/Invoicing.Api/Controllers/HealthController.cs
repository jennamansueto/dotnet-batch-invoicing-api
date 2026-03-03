using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Contoso.Invoicing.Api.Controllers
{
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(new { status = "OK" });
        }
    }
}
