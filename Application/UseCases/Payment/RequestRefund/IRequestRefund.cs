using FluentResults;
using MediatR;

namespace Application.UseCases.Transaction.RequestRefund;

public interface IRequestRefund : IRequestHandler<RequestRefundInput, Result<RequestRefundOutput>>
{
}
