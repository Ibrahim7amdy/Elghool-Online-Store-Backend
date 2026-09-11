using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetCodeAsync(string toEmail, string code);
    }
}
