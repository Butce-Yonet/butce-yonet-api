using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly IMediator _mediator;

        public BaseController(IMediator mediator) => this._mediator = mediator;

        [NonAction]
        public IActionResult Response(BaseResponse response)
        {
            return (IActionResult) this.StatusCode((int) response.StatusCode, (object) response);
        }
    }
}
