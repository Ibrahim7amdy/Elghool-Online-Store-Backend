using System;
using System.Collections.Generic;
using System.Text;

using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAdminRepository
{
    Task<Admin?> GetByEmailAsync(string email);
    Task<Admin?> GetByIdAsync(int id);
    Task AddAsync(Admin admin);
    Task<bool> EmailExistsAsync(string email);
    Task UpdateAsync(Admin admin);
}