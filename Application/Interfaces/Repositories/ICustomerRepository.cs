using Application.DTOs.Customer;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByPhoneNumberAsync(string phoneNumber);
    Task<Customer?> GetByIdAsync(int id);
    Task AddAsync(Customer customer);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> PhoneNumberExistsAsync(string phoneNumber);
    Task<bool> PhoneNumberExistsForOtherCustomerAsync(int customerId, string phoneNumber);
    Task UpdateAsync(Customer customer);
    Task<Customer?> GetByGoogleIdAsync(string googleId);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<IEnumerable<Customer>> GetByBranchAsync(int branchId);
    Task<IEnumerable<Customer>> GetNewThisMonthAsync();
    Task<IEnumerable<Customer>> GetTopSpendersAsync(int count = 10);
    Task<IEnumerable<Customer>> SearchAsync(string keyword);
    Task<IEnumerable<Customer>> FuzzySearchAsync(string keyword);
    Task ToggleStatusAsync(int customerId);
}