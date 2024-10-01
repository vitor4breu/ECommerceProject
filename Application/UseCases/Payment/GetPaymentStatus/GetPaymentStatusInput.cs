using FluentResults;
using MediatR;

namespace Application.UseCases.Transaction.GetTransactionStatus;

public record GetTransactionStatusInput(Guid OrderId) : IRequest<Result<GetTransactionStatusOutput>>;
