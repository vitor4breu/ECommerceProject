namespace Application.UseCases.Transaction.MakeTransaction;

public record MakeTransactionOutput(Guid TransactionId, bool IsSucess);