using FluentResults;
using MediatR;

namespace Application.UseCases.Transaction.MakeTransaction;

public interface IMakeTransaction : IRequestHandler<MakeTransactionInput, Result<MakeTransactionOutput>>
{
}
