using FluentResults;
using MediatR;

namespace Application.UseCases.Transaction.RequestRefund;

public record RequestRefundInput(Guid OrderId) : IRequest<Result<RequestRefundOutput>>;
