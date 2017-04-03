using System;

namespace TransactionUtilities.Tests.Example
{
    public interface IOrganizationRepository
    {
        void CreateOrganization(OrganizationModel organization);

        void DeleteOrganization(Guid id);
    }
}
