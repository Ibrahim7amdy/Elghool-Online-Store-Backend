// Application/Interfaces/Security/IGoogleAuthService.cs
namespace Application.Interfaces.Security;

public interface IGoogleAuthService
{
    Task<GoogleUserInfo> VerifyTokenAsync(string idToken);
}