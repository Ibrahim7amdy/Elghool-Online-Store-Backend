using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public class CustomerAuthResponseDto
    {
        public string? Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public int? PreferredBranchId { get; set; }
        public string? PhoneNumber { get; set; }

        public int CustomerId { get; set; }

        public bool RequiresAdditionalInfo { get; set; } = false;

    }
}
