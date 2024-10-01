using Microsoft.AspNetCore.Mvc;
using Application.UseCases.Item;
using MediatR;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Retrieves the list of items.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns the list of items or a bad request with errors.</returns>
    [HttpGet]
    public async Task<IActionResult> ListItems(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ListItemsInput(), cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors.Select(e => e.Message).ToList());
    }
}