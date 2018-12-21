using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TransactionUtilities.Tests
{
    [TestClass]
    public class SagaTests
    {
        [TestMethod]
        public void TestMultipleCommits()
        {
            // Arrange
            Mock<Action> compensation0 = new Mock<Action>();
            compensation0.Setup(m => m());

            Mock<Action> compensation1 = new Mock<Action>();
            compensation0.Setup(m => m());

            Mock<Action> compensation2 = new Mock<Action>();
            compensation0.Setup(m => m());

            Exception exception = null;

            // Act
            try
            {
                using (Saga saga = new Saga())
                {
                    DoAction0();
                    saga.RegisterCompensation(compensation0.Object);

                    saga.Commit();

                    DoAction1();
                    saga.RegisterCompensation(compensation1.Object);

                    DoAction2();
                    saga.RegisterCompensation(compensation2.Object);

                    saga.Commit();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.IsInstanceOfType(exception, typeof(ApplicationException));
            compensation0.Verify(m => m(), Times.Never);
            compensation1.Verify(m => m(), Times.Once);
            compensation2.Verify(m => m(), Times.Never);
        }

        private void DoAction0()
        {
        }

        private void DoAction1()
        {
        }

        private void DoAction2()
        {
            throw new ApplicationException();
        }
    }
}
