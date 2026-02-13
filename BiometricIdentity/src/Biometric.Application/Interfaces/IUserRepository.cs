using Biometric.Domain.Entities;

namespace Biometric.Application.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);

        // Performance optimization: Get only the FaceEmbedding for authentication
        Task<User?> FindNearestMatchAsync(float[] queryEmbedding, double threshold = 0.6);
    }
}
