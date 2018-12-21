using System;
using System.Collections.Generic;

namespace TransactionUtilities
{
    public class Saga : IDisposable
    {
        private List<Action> _compensations;

        /// <summary>
        /// Register a compensation action that will be run in case of rolling back the saga.
        /// </summary>
        /// <param name="compensation">The compensation action.</param>
        public void RegisterCompensation(Action compensation)
        {
            if (compensation == null)
            {
                throw new ArgumentNullException(nameof(compensation));
            }

            if (_compensations == null)
            {
                _compensations = new List<Action>();
            }

            _compensations.Add(compensation);
        }

        /// <summary>
        /// Commits the saga. Commit can be called multiple times during the saga.
        /// All non-committed actions will be rolled back on Dispose() call.
        /// Dispose() will call all compensation actions registered after the last Commit() call.
        /// </summary>
        public void Commit()
        {
            _compensations = null;
        }

        /// <summary>
        /// Rolls back all non-committed actions. Calls all compensation actions registered after the last Commit() call. 
        /// </summary>
        public void Dispose()
        {
            if (_compensations != null)
            {
                for (int i = _compensations.Count - 1; i >= 0; i--)
                {
                    _compensations[i]();
                }
            }
        }
    }
}
