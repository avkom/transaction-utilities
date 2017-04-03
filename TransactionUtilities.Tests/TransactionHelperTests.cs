using System;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestAddTransactionRollbackHandlerThrowsNoCurrentTransaction()
        {
            // Arrange
            Action handler = () => { };
            
            // Act
            _transactionHelper.AddTransactionRollbackHandler(handler);
        }

        [TestMethod]
        public void TestHandlerIsNotCalledWhenTransactionComplete()
        {
            // Arrange
            Mock<Action> mockHandler = new Mock<Action>();
            mockHandler.Setup(m => m());

            // Act
            using (var transactionScope = new TransactionScope())
            {
                _transactionHelper.AddTransactionRollbackHandler(mockHandler.Object);
                transactionScope.Complete();
            }

            // Assert
            mockHandler.Verify(m => m(), Times.Never);
        }

        [TestMethod]
        public void TestHandlerIsCalledWhenTransactionNonComplete()
        {
            // Arrange
            Mock<Action> mockHandler = new Mock<Action>();
            mockHandler.Setup(m => m());

            // Act
            using (new TransactionScope())
            {
                _transactionHelper.AddTransactionRollbackHandler(mockHandler.Object);
            }

            // Assert
            mockHandler.Verify(m => m(), Times.Once);
        }

        [TestMethod]
        public void TestHandlersAreCalledInReverseOrder()
        {
            // Arrange
            var mockSequence = new MockSequence();

            Mock<Action> mockHandler2 = new Mock<Action>(MockBehavior.Strict);
            mockHandler2.InSequence(mockSequence).Setup(m => m());

            Mock<Action> mockHandler1 = new Mock<Action>(MockBehavior.Strict);
            mockHandler1.InSequence(mockSequence).Setup(m => m());

            // Act
            using (new TransactionScope())
            {
                _transactionHelper.AddTransactionRollbackHandler(mockHandler1.Object);
                _transactionHelper.AddTransactionRollbackHandler(mockHandler2.Object);
            }

            // Assert
            mockHandler2.Verify(m => m(), Times.Once);
            mockHandler1.Verify(m => m(), Times.Once);
        }
    }
}
