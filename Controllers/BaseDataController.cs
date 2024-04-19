using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountManagermnet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BaseDataController : ControllerBase
    {
    }
}
