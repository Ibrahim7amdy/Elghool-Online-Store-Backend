using Application.DTOs.Auth;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class PasswordResetController : ControllerBase
{
    private readonly PasswordResetService _passwordResetService;

    public PasswordResetController(PasswordResetService passwordResetService)
    {
        _passwordResetService = passwordResetService;
    }


    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        try
        {
            await _passwordResetService.SendResetCodeAsync(dto);
            return Ok(new { message = "تم إرسال الكود على إيميلك" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    
    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyResetCodeDto dto)
    {
        try
        {
            await _passwordResetService.VerifyResetCodeAsync(dto);
            return Ok(new { message = "الكود صحيح، يمكنك الآن تغيير الباسورد" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [HttpPost("complete-reset-password")]
    public async Task<IActionResult> CompleteResetPassword([FromBody] CompleteResetPasswordDto dto, [FromQuery] string code)
    {
        try
        {
            await _passwordResetService.CompleteResetPasswordAsync(dto, code);
            return Ok(new { message = "تم تغيير الباسورد بنجاح" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}