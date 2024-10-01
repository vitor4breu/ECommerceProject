using Application.UseCases.Transaction.GetTransactionStatus;
using Application.UseCases.Transaction.MakeTransaction;
using Application.UseCases.Transaction.RequestRefund;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Initiates a new transaction.
    /// </summary>
    /// <param name="input">The transaction details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns the created transaction or a bad request with errors.</returns>
    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> MakeTransaction(MakeTransactionInput input, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(input, cancellationToken);
        if (result.IsSuccess)
            return CreatedAtAction(nameof(MakeTransaction), new { id = result.Value.TransactionId }, result.Value);

        return BadRequest(result.Errors.Select(e => e.Message).ToList());
    }

    /// <summary>
    /// Gets the status of a transaction by its ID.
    /// </summary>
    /// <param name="id">The transaction ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns the transaction status or a bad request with errors.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTransactionStatus(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTransactionStatusInput(id), cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors.Select(e => e.Message).ToList());
    }

    /// <summary>
    /// Requests a refund for a transaction.
    /// </summary>
    /// <param name="id">The transaction ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns the refund result or a bad request with errors.</returns>
    [HttpPost("{id:guid}/refund")]
    public async Task<IActionResult> RequestRefund(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RequestRefundInput(id), cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors.Select(e => e.Message).ToList());
    }
}