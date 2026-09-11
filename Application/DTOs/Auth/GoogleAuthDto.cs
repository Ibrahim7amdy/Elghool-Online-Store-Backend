using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public class GoogleAuthDto
    {
        public string IdToken { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int? PreferredBranchId { get; set; }
    }
}
