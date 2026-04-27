using ButceYonet.Application.Application.Features.Landing.GetLandingStats;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers;

[AllowAnonymous]
public class LandingController : BaseController
{
    public LandingController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Landing page için platform istatistiklerini döner. Yanıtlar 15 dakika önbelleklenir.
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(BaseResponse<LandingStatsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        var response = await _mediator.Send(new GetLandingStatsQuery());
        return Response(response);
    }
}