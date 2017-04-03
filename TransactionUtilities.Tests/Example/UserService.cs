using System;
using System.Transactions;

namespace TransactionUtilities.Tests.Example
{
    public class UserService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IBillingRepository _billingRepository;
        private readonly ITransactionHelper _transactionHelper;

        public UserService(
            IOrganizationRepository organizationRepository,
            IUserProfileRepository userProfileRepository,
            IBillingRepository billingRepository,
            ITransactionHelper transactionHelper)
        {
            _organizationRepository = organizationRepository;
            _userProfileRepository = userProfileRepository;
            _billingRepository = billingRepository;
            _transactionHelper = transactionHelper;
        }

        public void RegisterUser(UserModel user)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                if (user.Organization.Id.Equals(Guid.Empty))
                {
                    user.Organization.Id = Guid.NewGuid();
                    _organizationRepository.CreateOrganization(user.Organization);
                    _transactionHelper.AddTransactionRollbackHandler(
                        () => _organizationRepository.DeleteOrganization(user.Organization.Id));
                }

                user.Id = Guid.NewGuid();

                _userProfileRepository.CreateUserProfile(user);
                _transactionHelper.AddTransactionRollbackHandler(
                    () => _userProfileRepository.DeleteUserProfile(user.Id));

                _billingRepository.CreateUserAccount(user);

                transactionScope.Complete();
            }
        }
    }
}
