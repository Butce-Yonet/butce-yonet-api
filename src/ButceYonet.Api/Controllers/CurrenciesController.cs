using ButceYonet.Application.Application.Features.Currencies.GetCurrencies;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers
{
    public class CurrenciesController : BaseController
    {
        private ILogger<CurrenciesController> _logger;
        public CurrenciesController(IMediator mediator, ILogger<CurrenciesController> logger) : base(mediator)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sistem tarafından desteklenen para birimlerini getirir
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<CurrencyDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<CurrencyDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<CurrencyDto>>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<CurrencyDto>>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<CurrencyDto>>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromQuery] GetCurrencyQuery request)
        {
            var response = await _mediator.Send(request);

            return Response(response);
        }
    }
}
