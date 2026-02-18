using Biometric.Domain.Entities;

namespace Biometric.Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
