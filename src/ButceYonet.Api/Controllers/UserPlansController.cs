using ButceYonet.Application.Application.Features.Plans.GetPlans;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers
{
    public class UserPlansController : BaseController
    {
        public UserPlansController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Kullanıcının kullanabileceği paketlerin bir listesini getirir
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<PlanDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<PlanDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<PlanDto>>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<PlanDto>>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<PlanDto>>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            var response = await _mediator.Send(new GetPlanQuery());
            return Response(response);
        }
    }
}
