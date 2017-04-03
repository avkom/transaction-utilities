using System;
using System.Collections.Generic;
using System.Transactions;

namespace TransactionUtilities
{
    public class TransactionHelper : ITransactionHelper, IEnlistmentNotification
    {
        private List<Action> _rollbackHandlers;

        public void AddTransactionRollbackHandler(Action handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (Transaction.Current == null)
            {
                throw new InvalidOperationException("There is no current transaction to enlist.");
            }

            if (_rollbackHandlers == null)
            {
                _rollbackHandlers = new List<Action>();
                Transaction.Current.EnlistVolatile(this, EnlistmentOptions.None);
            }

            _rollbackHandlers.Add(handler);
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        public void Commit(Enlistment enlistment)
        {
            enlistment.Done();
        }

        public void Rollback(Enlistment enlistment)
        {
            if (_rollbackHandlers != null)
            {
                _rollbackHandlers.Reverse();
                _rollbackHandlers.ForEach(handler => handler());
                _rollbackHandlers = null;
            }

            enlistment.Done();
        }

        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }
    }
}