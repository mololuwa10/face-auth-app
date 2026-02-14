using Pgvector;

namespace Biometric.Domain.Entities
{
    public class User
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }

        // Mapping to this to 'vector' type in the database
        public Vector? FaceEmbedding { get; set; }

        // Audit traits
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; set; }
    }
}
