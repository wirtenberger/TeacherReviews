using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using TeacherReviews.API.Contracts.Repositories;
using TeacherReviews.Domain.Entities;
using TeacherReviews.Domain.Exceptions;

namespace TeacherReviews.API.Services;

public class AdminUserService
{
    private readonly IAdminUserRepository _adminUserRepository;

    private readonly PasswordHasher<AdminUser> _passwordHasher = new();

    private readonly IConfiguration _configuration;

    public AdminUserService(IAdminUserRepository adminUserRepository, IConfiguration configuration)
    {
        _adminUserRepository = adminUserRepository;
        _configuration = configuration;
    }

    public Task<List<AdminUser>> GetAllAsync(Expression<Func<AdminUser, bool>>? filter)
    {
        return _adminUserRepository.GetAllAsync(filter);
    }

    public async Task<AdminUser> GetById(int id)
    {
        var user = await _adminUserRepository.GetByIdAsync(id);
        if (user is null)
        {
            throw new EntityNotFoundException(typeof(AdminUser), id.ToString());
        }

        return user;
    }

    public async Task<AdminUser> GetByUsername(string username)
    {
        var user = await _adminUserRepository.GetByUsernameAsync(username);
        if (user is null)
        {
            throw new EntityNotFoundException(typeof(AdminUser), username);
        }

        return user;
    }

    public async Task<AdminUser> CreateAsync(AdminUser user)
    {
        if (await _adminUserRepository.GetByUsernameAsync(user.Username) is null)
        {
            throw new EntityExistsException(typeof(AdminUser), nameof(AdminUser.Username), user.Username);
        }

        user.Password = _passwordHasher.HashPassword(user, user.Password);
        return await _adminUserRepository.CreateAsync(user);
    }

    public async Task<AdminUser> UpdateAsync(AdminUser user)
    {
        if (await _adminUserRepository.GetByIdAsync(user.Id) is null)
        {
            throw new EntityNotFoundException(typeof(AdminUser), user.Id.ToString());
        }

        var existing = await _adminUserRepository.GetByUsernameAsync(user.Username);
        if (existing is not null && existing.Username == user.Username && existing.Id != user.Id)
        {
            throw new EntityExistsException(typeof(AdminUser), nameof(AdminUser.Username), user.Username);
        }

        user.Password = _passwordHasher.HashPassword(user, user.Password);

        return await _adminUserRepository.UpdateAsync(user);
    }

    public async Task<AdminUser> DeleteAsync(int id)
    {
        if (await _adminUserRepository.GetByIdAsync(id) is null)
        {
            throw new EntityNotFoundException(typeof(AdminUser), id.ToString());
        }

        return await _adminUserRepository.DeleteAsync(id);
    }

    public async Task<bool> IsAuthenticated(string username, string password)
    {
        var user = await _adminUserRepository.GetByUsernameAsync(username);
        if (user is not null)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success
                   || result == PasswordVerificationResult.SuccessRehashNeeded;
        }

        var (mainUsername, mainPassword) = (
            _configuration["Secrets:MainAdminUser:Username"],
            _configuration["Secrets:MainAdminUser:Password"]
        );

        return mainUsername is null || mainPassword is null || (mainUsername == username && mainPassword == password);
    }
}