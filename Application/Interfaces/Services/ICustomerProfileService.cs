using Application.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface ICustomerProfileService
    {
        Task UpdateProfileAsync(int customerId, UpdateCustomerProfileDto dto);
        Task<CustomerProfileDto> GetProfileAsync(int customerId);
        Task ChangePasswordAsync(int customerId, ChangePasswordDto dto);
    }
}
