using ButceYonet.Application.Application.Features.Notebooks.DeleteNotebook;
using ButceYonet.Application.Application.Features.Notebooks.GetNotebook;
using ButceYonet.Application.Application.Features.Notebooks.GetNotebooks;
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
        /// Kullanıcının hesap defterini silmek için kullanılır (yalnızca işlemi olmayan defterler silinebilir)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteNotebookCommand
            {
                Id = id
            };

            var response = await _mediator.Send(command);

            return Response(response);
        }

        /// <summary>
        /// Kullanıcının hesap defterlerini (aylık dönemlerini) getirmek için kullanılır
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
