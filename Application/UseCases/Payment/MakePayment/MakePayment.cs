using Domain.Repository;
using FluentResults;
using Application.Interfaces;
using MediatR;

namespace Application.UseCases.Transaction.MakeTransaction;

public class MakeTransaction : IMakeTransaction
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMediator _mediator;
    private readonly ITransactionProcessor _transactionGateway;

    public MakeTransaction(IOrderRepository orderRepository, ITransactionRepository transactionRepository, IMediator mediator, ITransactionProcessor transactionGateway)
    {
        _orderRepository = orderRepository;
        _transactionRepository = transactionRepository;
        _mediator = mediator;
        _transactionGateway = transactionGateway;
    }

    public async Task<Result<MakeTransactionOutput>> Handle(MakeTransactionInput request, CancellationToken cancellationToken)
    {
        try
        {
            //Do order

            return Result.Ok(new MakeTransactionOutput(new Guid(), true));
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}
