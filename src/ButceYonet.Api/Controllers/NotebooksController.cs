using ButceYonet.Application.Application.Features.Notebooks.CreateNotebook;
using ButceYonet.Application.Application.Features.Notebooks.DeleteNotebook;
using ButceYonet.Application.Application.Features.Notebooks.GetNotebook;
using ButceYonet.Application.Application.Features.Notebooks.GetNotebooks;
using ButceYonet.Application.Application.Features.Notebooks.UpdateNotebook;
using ButceYonet.Application.Application.Shared.Dtos;
using DotBoil.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ButceYonet.Api.Controllers
{
    public class NotebooksController : BaseController
    {
        public NotebooksController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Kullanıcı hesaplarını tutmak için yeni bir defter oluşturması amacıyla kullanılır
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateNotebookCommand command)
        {
            var response = await _mediator.Send(command);

            return Response(response);
        }

        /// <summary>
        /// Kullanıcının hesap defterinin adını güncellemek için kullanılır
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateNotebookCommand command)
        {
            var response = await _mediator.Send(command);

            return Response(response);
        }

        /// <summary>
        /// Kullanıcının hesap defterini silmek için kullanılır
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromBody] DeleteNotebookCommand command)
        {
            var response = await _mediator.Send(command);
            
            return Response(response);
        }
        
        /// <summary>
        /// Kullanıcının hesap defterlerini getirmek için kullanılır
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotebookDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<NotebookDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<NotebookDto>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(IEnumerable<NotebookDto>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(IEnumerable<NotebookDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var request = new GetNotebooksQuery();
            var response = await _mediator.Send(request);

            return Response(response);
        }

        /// <summary>
        /// Kullanıcının seçtiği defteri getirmek için kullanılır
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NotebookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotebookDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotebookDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotebookDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotebookDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var request = new GetNotebookQuery(id);
            var response = await _mediator.Send(request);

            return Response(response);
        }
    }
}
