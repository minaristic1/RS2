using System.Threading.Tasks;

using UserService.Application.DTOs;

namespace UserService.Application.Interfaces
{
    public interface IUserAppService
    {
        Task<UserResponse?> RegisterAsync(RegisterUserRequest request);

        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}