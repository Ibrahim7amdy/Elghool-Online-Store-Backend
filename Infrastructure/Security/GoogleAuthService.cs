using Application.Interfaces.Security;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Security;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IConfiguration _configuration;

    public GoogleAuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<GoogleUserInfo> VerifyTokenAsync(string idToken)
    {
        var clientId = _configuration["Google:ClientId"];
        if (string.IsNullOrWhiteSpace(clientId))
        {
            throw new InvalidOperationException(
                "Google ClientId is missing.");
        }

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { clientId }
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

        return new GoogleUserInfo
        {
            Email = payload.Email,
            FirstName = payload.GivenName ?? string.Empty,
            LastName = payload.FamilyName ?? string.Empty,
            GoogleId = payload.Subject
        };
    }
}