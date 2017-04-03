using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace TransactionUtilities.Tests.Example
{
    [TestClass]
    public class UserServiceTests
    {
        private TransactionHelper _transactionHelper;
        private Mock<IOrganizationRepository> _mockOrganizationRepository;
        private Mock<IUserProfileRepository> _mockUserProfileRepository;
        private Mock<IBillingRepository> _mockBillingRepository;
        private UserService _userService;

        [TestInitialize]
        public void InitializeTest()
        {
            _transactionHelper = new TransactionHelper();
            _mockOrganizationRepository = new Mock<IOrganizationRepository>();
            _mockUserProfileRepository = new Mock<IUserProfileRepository>();
            _mockBillingRepository = new Mock<IBillingRepository>();

            _userService = new UserService(
                _mockOrganizationRepository.Object,
                _mockUserProfileRepository.Object,
                _mockBillingRepository.Object,
                _transactionHelper);
        }

        [TestMethod]
        public void TestWhenFailsToCreateOrganization()
        {
            // Arrange
            _mockOrganizationRepository.Setup(m => m.CreateOrganization(It.IsAny<OrganizationModel>())).Throws<Exception>();
            _mockOrganizationRepository.Setup(m => m.DeleteOrganization(It.IsAny<Guid>()));

            _mockUserProfileRepository.Setup(m => m.CreateUserProfile(It.IsAny<UserModel>()));
            _mockUserProfileRepository.Setup(m => m.DeleteUserProfile(It.IsAny<Guid>()));

            _mockBillingRepository.Setup(m => m.CreateUserAccount(It.IsAny<UserModel>()));

            UserModel user = new UserModel();
            Exception exception = null;

            // Act
            try
            {
                _userService.RegisterUser(user);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.IsInstanceOfType(exception, typeof(Exception));

            _mockOrganizationRepository.Verify(m => m.CreateOrganization(It.IsAny<OrganizationModel>()), Times.Once);
            _mockOrganizationRepository.Verify(m => m.DeleteOrganization(It.IsAny<Guid>()), Times.Never);

            _mockUserProfileRepository.Verify(m => m.CreateUserProfile(It.IsAny<UserModel>()), Times.Never);
            _mockUserProfileRepository.Verify(m => m.DeleteUserProfile(It.IsAny<Guid>()), Times.Never);

            _mockBillingRepository.Verify(m => m.CreateUserAccount(It.IsAny<UserModel>()), Times.Never);
        }

        [TestMethod]
        public void TestWhenFailsToCreateProfile()
        {
            // Arrange
            _mockOrganizationRepository.Setup(m => m.CreateOrganization(It.IsAny<OrganizationModel>()));
            _mockOrganizationRepository.Setup(m => m.DeleteOrganization(It.IsAny<Guid>()));

            _mockUserProfileRepository.Setup(m => m.CreateUserProfile(It.IsAny<UserModel>())).Throws<Exception>();
            _mockUserProfileRepository.Setup(m => m.DeleteUserProfile(It.IsAny<Guid>()));

            _mockBillingRepository.Setup(m => m.CreateUserAccount(It.IsAny<UserModel>()));

            UserModel user = new UserModel();
            Exception exception = null;

            // Act
            try
            {
                _userService.RegisterUser(user);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.IsInstanceOfType(exception, typeof(Exception));

            _mockOrganizationRepository.Verify(m => m.CreateOrganization(It.IsAny<OrganizationModel>()), Times.Once);
            _mockOrganizationRepository.Verify(m => m.DeleteOrganization(It.IsAny<Guid>()), Times.Once);

            _mockUserProfileRepository.Verify(m => m.CreateUserProfile(It.IsAny<UserModel>()), Times.Once);
            _mockUserProfileRepository.Verify(m => m.DeleteUserProfile(It.IsAny<Guid>()), Times.Never);

            _mockBillingRepository.Verify(m => m.CreateUserAccount(It.IsAny<UserModel>()), Times.Never);
        }
    }
}
