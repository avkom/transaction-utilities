using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TransactionUtilities.Tests
{
    [TestClass]
    public class TransactionHelperTests
    {
        private TransactionHelper _transactionHelper;

        [TestInitialize]
        public void InitializeTest()
        {
            _transactionHelper = new TransactionHelper();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddTransactionRollbackHandlerThrowsWhenHandlerIsNull()
        {
            // Act
            _transactionHelper.AddTransactionRollbackHandler(null);
        }
    }
}
