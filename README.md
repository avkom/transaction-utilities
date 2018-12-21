# TransactionHelper

Provides an implementation of *Compensation* pattern.

`OrganizationRepository`, `UserProfileRepository`, and `BillingRepository` are gateways to three different databases, which do not support distributed transaction, so we use *Compensation* pattern. Firstly, we create an organization. Secondly, we create a user profile. Finally, we create a billing account for the user. If one of the operations fails, we revert only actions performed *before* the failing operation. In our example, we revert actions by removing created objects. Note: we rollback operations in a *reverse* order: the last successful operation will be reverted first.

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

# Saga Pattern Implementation

In the following example, the method `DoStep4()` throws an exception, so `RollbackStep3()` and `RollbackStep2()` compensation methods will be called when exiting `using` statement. `RollbackStep1()` will not be called because `saga.Commit()` was called after registering the compensation action for the first step.
The private methods will be called in the following order:
1. `DoStep1();`
2. `DoStep2();`
3. `DoStep3();`
4. `DoStep4();` - this method throws an exception
5. `RollbackStep3();`
6. `RollbackStep2();`

```c#
public void DoSomeComplexAction()
{
    using (Saga saga = new Saga())
    {
        DoStep1();
        saga.RegisterCompensation(RollbackStep1);

        saga.Commit();

        DoStep2();
        saga.RegisterCompensation(RollbackStep2);

        DoStep3();
        saga.RegisterCompensation(RollbackStep3);

        DoStep4();
        saga.RegisterCompensation(RollbackStep4);

        saga.Commit();
    }

    private void DoStep1() {}

    private void RollbackStep1() {}

    private void DoStep2() {}

    private void RollbackStep2() {}

    private void DoStep3() {}

    private void RollbackStep3() {}

    private void DoStep4() { throw new ApplicationException(); }

    private void RollbackStep4() {}
}
```
