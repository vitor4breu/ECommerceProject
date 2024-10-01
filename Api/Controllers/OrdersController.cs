using Application.UseCases.Order.CancelOrder;
using Application.UseCases.Order.CreateOrder;
using Application.UseCases.Order.GetOrderStatus;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="input">The order details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns the created order or a bad request with errors.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderInput input, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(input, cancellationToken);

        if (result.IsSuccess)
            return CreatedAtAction(nameof(CreateOrder), new { id = result.Value.OrderId }, result.Value);

        return BadRequest(result.Errors.Select(e => e.Message));
    }

    /// <summary>
    /// Cancels an existing order by its ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns no content if successful or a bad request with errors.</returns>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelOrderInput(id), cancellationToken);

        if (result.IsSuccess)
            return NoContent();

        return BadRequest(result.Errors.Select(e => e.Message));

    }

    /// <summary>
    /// Gets the status of an order by its ID.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns the order status or a bad request with errors.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderStatus(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrderStatusInput(id), cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors.Select(e => e.Message));
    }
}