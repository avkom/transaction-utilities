using System;

namespace TransactionUtilities
{
    public interface ITransactionHelper
    {
        void AddTransactionRollbackHandler(Action handler);
    }
}
