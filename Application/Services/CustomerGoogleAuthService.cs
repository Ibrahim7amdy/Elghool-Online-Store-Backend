using Application.DTOs.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Domain.Entities;

namespace Application.Services;

public class CustomerGoogleAuthService
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ITokenService _tokenService;

    public CustomerGoogleAuthService(
        IGoogleAuthService googleAuthService,
        ICustomerRepository customerRepository,
        IBranchRepository branchRepository,
        ITokenService tokenService)
    {
        _googleAuthService = googleAuthService;
        _customerRepository = customerRepository;
        _branchRepository = branchRepository;
        _tokenService = tokenService;
    }

    public async Task<CustomerAuthResponseDto> LoginWithGoogleAsync(GoogleAuthDto dto)
    {
        // تتحقق من التوكن مع Google
        var googleUser = await _googleAuthService.VerifyTokenAsync(dto.IdToken);

        // دور على الـ customer
        var customer = await _customerRepository.GetByGoogleIdAsync(googleUser.GoogleId);
        if (customer == null)
            customer = await _customerRepository.GetByEmailAsync(googleUser.Email);

        // مستخدم موجود → سجل دخول مباشرة
        if (customer != null)
        {
            // لو موجود بالإيميل بس ومش مربوط بـ Google
            if (customer.GoogleId == null)
            {
                customer.GoogleId = googleUser.GoogleId;
                await _customerRepository.UpdateAsync(customer);
            }

            return new CustomerAuthResponseDto
            {
                RequiresAdditionalInfo = false,
                Token = _tokenService.GenerateCustomerToken(customer),
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber,
                PreferredBranchId = customer.PreferredBranchId,
                CustomerId = customer.Customer_Id
            };
        }

        // مستخدم جديد → لو البيانات الناقصة مش موجودة
        if (string.IsNullOrEmpty(dto.PhoneNumber) || !dto.PreferredBranchId.HasValue)
        {
            return new CustomerAuthResponseDto
            {
                RequiresAdditionalInfo = true,
                Token = null,
                Email = googleUser.Email,
                FirstName = googleUser.FirstName,
                LastName = googleUser.LastName

            };
        }

        // تتأكد إن الفرع موجود
        var branchExists = await _branchRepository.ExistsAsync(dto.PreferredBranchId.Value);
        if (!branchExists)
            throw new Exception("الفرع المختار مش موجود");

        // تتأكد إن الموبايل مش مسجل
        if (await _customerRepository.PhoneNumberExistsAsync(dto.PhoneNumber))
            throw new Exception("رقم الموبايل ده مسجل قبل كده");

        // سجل المستخدم الجديد
        customer = new Customer
        {
            FirstName = googleUser.FirstName,
            LastName = googleUser.LastName,
            Email = googleUser.Email,
            GoogleId = googleUser.GoogleId,
            IsExternalLogin = true,
            PhoneNumber = dto.PhoneNumber,
            PreferredBranchId = dto.PreferredBranchId,
            PasswordHash = string.Empty
        };

        await _customerRepository.AddAsync(customer);

        return new CustomerAuthResponseDto
        {
            RequiresAdditionalInfo = false,
            Token = _tokenService.GenerateCustomerToken(customer),
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            PreferredBranchId = customer.PreferredBranchId,
            CustomerId = customer.Customer_Id
        };
    }
}