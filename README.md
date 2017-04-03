# TransactionHelper

Provides an implementation of *Compensation* pattern.

`OrganizationRepository`, `UserProfileRepository`, and `BillingRepository` are gateways to three different databases, which do not support distributed transaction, so we use *Compensation* pattern. Firstly, we create an organization. Secondly, we create a user profile. Finally, we create a billing account for the user. If one of the operations fails, we revert actions performed before the failing operation. In our example, we revert actions by removing created objects. Note: we rollback operations in a reverse order: the last successful operation will be reverted first.

```c#
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
```
