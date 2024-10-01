using FluentResults;
using MediatR;

namespace Application.UseCases.Transaction.GetTransactionStatus;

public interface IGetTransactionStatus : IRequestHandler<GetTransactionStatusInput, Result<GetTransactionStatusOutput>>
{
}
