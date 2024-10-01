using Application.Interfaces;
using Domain.Enum;

namespace Application.UseCases.Transaction.TransactionMethod;

public class TransactionFactory
{
    public static ITransaction CreateTransaction(TransactionMethodEnum method, ITransactionProcessor transactionGateway)
    {
        return method switch
        {
            TransactionMethodEnum.Pix => new PixTransaction(transactionGateway),
            TransactionMethodEnum.Credito => new CreditoTransaction(transactionGateway),
            TransactionMethodEnum.Debito => new DebitoTransaction(transactionGateway),
            TransactionMethodEnum.Boleto => new BoletoTransaction(transactionGateway),
            _ => throw new ArgumentException("Invalid transaction method")
        };
    }
}
