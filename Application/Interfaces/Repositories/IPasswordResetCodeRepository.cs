using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Repositories
{
    public interface IPasswordResetCodeRepository
    {
        Task AddAsync(PasswordResetCode code);
        Task<PasswordResetCode?> GetValidCodeAsync(string email, string code);
        Task DeleteAllByEmailAsync(string email);
    }
}
