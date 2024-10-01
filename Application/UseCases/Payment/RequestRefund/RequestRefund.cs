using Application.Interfaces;
using Application.UseCases.Transaction.TransactionMethod;
using Domain.Entity;
using Domain.Enum;
using Domain.Event;
using Domain.Repository;
using FluentResults;
using MediatR;

namespace Application.UseCases.Transaction.RequestRefund;

public class RequestRefund : IRequestRefund
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMediator _mediator;
    private readonly ITransactionProcessor _transactionGateway;

    public RequestRefund(IOrderRepository orderRepository, ITransactionRepository transactionRepository, IMediator mediator, ITransactionProcessor transactionGateway)
    {
        _orderRepository = orderRepository;
        _transactionRepository = transactionRepository;
        _mediator = mediator;
        _transactionGateway = transactionGateway;
    }

    public async Task<Result<RequestRefundOutput>> Handle(RequestRefundInput request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
                return Result.Fail($"Order with ID {request.OrderId} not found.");

            var oldOrderState = order.State;

            order.RequestRefund();
            await DomainEvents.DispatchNotifications(_mediator);

            var transaction = await _transactionRepository.GetByOrderIdAsync(order.Id, cancellationToken);
            var transactionService = TransactionFactory.CreateTransaction(transaction.Method, _transactionGateway);

            if (transactionService.CanRefund())
            {
                await transactionService.Refund(transaction.Id);
                order.Refund();
                var transactionUpdate = new TransactionDomain(order.Id, order.Total, transaction.Method, TransactionStatusEnum.Reembolsado);
                await _transactionRepository.SaveAsync(transactionUpdate, cancellationToken);
                await _orderRepository.UpdateAsync(order, cancellationToken);
            }
            else
            {
                order.NonRefundable(oldOrderState);
            }

            await DomainEvents.DispatchNotifications(_mediator);

            return Result.Ok(new RequestRefundOutput(transactionService.CanRefund()));
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
