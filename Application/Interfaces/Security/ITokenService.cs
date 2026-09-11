using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Security;

public interface ITokenService
{
    string GenerateCustomerToken(Domain.Entities.Customer customer);
    string GenerateAdminToken(Domain.Entities.Admin admin);
}