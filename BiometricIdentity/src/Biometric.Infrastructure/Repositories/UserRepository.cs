using Biometric.Application.Interfaces;
using Biometric.Domain.Entities;
using Biometric.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Biometric.Infrastructure.Repositories
{
    public class UserRepository(ApplicationDbContext _context) : IUserRepository
    {
        private readonly ApplicationDbContext _context = _context;

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> FindNearestMatchAsync(float[] queryEmbedding, double threshold)
        {
            var vector = new Vector(queryEmbedding);

            // PILLAR 3: Use Cosine Distance (standard for Face Recognition)
            return await _context
                .Users.OrderBy(u => u.FaceEmbedding!.CosineDistance(vector))
                .Where(u => u.FaceEmbedding!.CosineDistance(vector) < threshold)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> FindBestMatchAsync(float[] queryEmbedding, double threshold = 0.9)
        {
            var queryVector = new Vector(queryEmbedding);

            return await _context
                .Users.Where(u => 1 - u.FaceEmbedding!.CosineDistance(queryVector) >= threshold)
                .OrderBy(u => u.FaceEmbedding!.CosineDistance(queryVector))
                .FirstOrDefaultAsync();
        }
    }
}
