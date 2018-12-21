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
            Mock<Action> rollbackStep1 = new Mock<Action>();
            rollbackStep1.Setup(m => m());

            Mock<Action> rollbackStep2 = new Mock<Action>();
            rollbackStep1.Setup(m => m());

            Mock<Action> rollbackStep3 = new Mock<Action>();
            rollbackStep1.Setup(m => m());

            Mock<Action> rollbackStep4 = new Mock<Action>();
            rollbackStep4.Setup(m => m());

            Exception exception = null;

            // Act
            try
            {
                using (Saga saga = new Saga())
                {
                    DoStep1();
                    saga.RegisterCompensation(rollbackStep1.Object);

                    saga.Commit();

                    DoStep2();
                    saga.RegisterCompensation(rollbackStep2.Object);

                    DoStep3();
                    saga.RegisterCompensation(rollbackStep3.Object);

                    DoStep4();
                    saga.RegisterCompensation(rollbackStep4.Object);

                    saga.Commit();
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.IsInstanceOfType(exception, typeof(ApplicationException));

            rollbackStep1.Verify(m => m(), Times.Never);
            rollbackStep2.Verify(m => m(), Times.Once);
            rollbackStep3.Verify(m => m(), Times.Once);
            rollbackStep4.Verify(m => m(), Times.Never);
        }

        private void DoStep1()
        {
        }

        private void DoStep2()
        {
        }

        private void DoStep3()
        {
        }

        private void DoStep4()
        {
            throw new ApplicationException();
        }
    }
}
