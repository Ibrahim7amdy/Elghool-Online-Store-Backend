using Application.DTOs.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Domain.Entities;

namespace Application.Services;

public class CustomerAuthService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IBranchRepository _branchRepository;

    public CustomerAuthService(
        ICustomerRepository customerRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IBranchRepository branchRepository)
    {
        _customerRepository = customerRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _branchRepository = branchRepository;
    }

    public async Task<CustomerAuthResponseDto> RegisterAsync(CustomerRegisterDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        if (await _customerRepository.EmailExistsAsync(normalizedEmail))
            throw new Exception("الإيميل ده مسجل قبل كده");

        if (await _customerRepository.PhoneNumberExistsAsync(dto.PhoneNumber))
            throw new Exception("رقم الموبايل ده مسجل قبل كده");

        if (dto.PreferredBranchId.HasValue)
        {
            var branchExists = await _branchRepository.ExistsAsync(dto.PreferredBranchId.Value);
            if (!branchExists)
                throw new Exception("الفرع المختار مش موجود");
        }

        var customer = new Customer
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = normalizedEmail,
            PasswordHash = _passwordHasher.Hash(dto.Password),
            PreferredBranchId = dto.PreferredBranchId
        };

        await _customerRepository.AddAsync(customer);

        return new CustomerAuthResponseDto
        {
            Token = _tokenService.GenerateCustomerToken(customer),
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PreferredBranchId = customer.PreferredBranchId,
            PhoneNumber = customer.PhoneNumber,
            CustomerId = customer.Customer_Id
        };
    }

    public async Task<CustomerAuthResponseDto> LoginAsync(CustomerLoginDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLowerInvariant();

        var customer = await _customerRepository.GetByEmailAsync(normalizedEmail);

        if (customer == null || !_passwordHasher.Verify(dto.Password, customer.PasswordHash))
            throw new Exception("الإيميل أو الباسورد غلط");

        return new CustomerAuthResponseDto
        {
            Token = _tokenService.GenerateCustomerToken(customer),
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PreferredBranchId = customer.PreferredBranchId,
            PhoneNumber = customer.PhoneNumber,
            CustomerId = customer.Customer_Id
        };
    }
}