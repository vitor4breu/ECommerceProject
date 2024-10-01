using Domain.Enum;

namespace Application.UseCases.Transaction.GetTransactionStatus;

public record GetTransactionStatusOutput(TransactionStatusEnum Status);