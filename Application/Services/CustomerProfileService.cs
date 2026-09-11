using Application.DTOs.Customer;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.Services;

namespace Application.Services;

public class CustomerProfileService : ICustomerProfileService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IPasswordHasher _passwordHasher;


    public CustomerProfileService(
        ICustomerRepository customerRepository,
        IBranchRepository branchRepository,
        IPasswordHasher passwordHasher) 
    {
        _customerRepository = customerRepository;
        _branchRepository = branchRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task UpdateProfileAsync(int customerId, UpdateCustomerProfileDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            throw new Exception("المستخدم مش موجود");

        // لو بعت موبايل جديد، تتحقق منه
        if (dto.PhoneNumber != null)
        {
            if (await _customerRepository.PhoneNumberExistsForOtherCustomerAsync(customerId, dto.PhoneNumber))
                throw new Exception("رقم الموبايل ده مسجل قبل كده");

            customer.PhoneNumber = dto.PhoneNumber;
        }

        // لو بعت اسم جديد، حدثه
        if (dto.FirstName != null)
            customer.FirstName = dto.FirstName;

        if (dto.LastName != null)
            customer.LastName = dto.LastName;


        if (dto.PreferredBranchId.HasValue) 
        {
            var branchExists = await _branchRepository.ExistsAsync(dto.PreferredBranchId.Value);
            if (!branchExists)
                throw new Exception("الفرع المختار غير موجود في السيستم");

            customer.PreferredBranchId = dto.PreferredBranchId.Value;
        }

        await _customerRepository.UpdateAsync(customer);
    }

    public async Task<CustomerProfileDto> GetProfileAsync(int customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            throw new Exception("المستخدم مش موجود");

        return new CustomerProfileDto
        {
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            Email = customer.Email,
            PreferredBranchId = customer.PreferredBranchId,
            CustomerId = customer.Customer_Id
        };
    }
    public async Task ChangePasswordAsync(int customerId, ChangePasswordDto dto)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            throw new Exception("المستخدم مش موجود");

        // لو External Login مش عنده باسورد
        if (customer.IsExternalLogin)
            throw new Exception("الحساب ده مرتبط بـ Google أو Facebook، مش فيه باسورد.");

        // تتحقق من الباسورد الحالي
        if (!_passwordHasher.Verify(dto.CurrentPassword, customer.PasswordHash!))
            throw new Exception("الباسورد الحالي غلط.");

        customer.PasswordHash = _passwordHasher.Hash(dto.NewPassword);
        await _customerRepository.UpdateAsync(customer);
    }
}