using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public class CompleteResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;

    }
}
