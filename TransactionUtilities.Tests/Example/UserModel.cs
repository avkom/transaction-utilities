using System;

namespace TransactionUtilities.Tests.Example
{
    public class UserModel
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public OrganizationModel Organization { get; set; }

        public UserModel()
        {
            Organization = new OrganizationModel();
        }
    }
}
