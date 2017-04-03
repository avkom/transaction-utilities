using System;

namespace TransactionUtilities.Tests.Example
{
    public interface IUserProfileRepository
    {
        void CreateUserProfile(UserModel user);

        void DeleteUserProfile(Guid userId);
    }
}
