using Domain.Enum;
using FluentResults;
using MediatR;

namespace Application.UseCases.Transaction.MakeTransaction;

public record MakeTransactionInput(Guid OrderId, TransactionMethodEnum Method) : IRequest<Result<MakeTransactionOutput>>;
