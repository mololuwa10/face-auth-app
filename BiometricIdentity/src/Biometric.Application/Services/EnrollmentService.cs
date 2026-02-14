using Biometric.Application.Interfaces;
using Biometric.Domain.Entities;
using Pgvector;

namespace Biometric.Application.Services
{
    public class EnrollmentService(IFaceAiService aiService, IUserRepository userRepository)
    {
        private readonly IFaceAiService _aiService = aiService;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<User> EnrollUserAsync(
            string firstName,
            string lastName,
            string email,
            Stream imageStream,
            string fileName
        )
        {
            // Check if user already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
                throw new Exception("A user with this email is already registered.");

            // Call Python AI to extract the 512-d embedding
            var aiResult = await _aiService.ExtractFaceEmbeddingAsync(imageStream, fileName);

            if (
                !aiResult.Success
                || aiResult.FaceEmbedding == null
                || aiResult.FaceEmbedding.Length == 0
            )
            {
                throw new Exception(
                    "AI Processing failed: The embedding returned was empty. Check if a face was detected."
                );
            }

            // Create the Domain Entity
            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                FaceEmbedding = new Vector(aiResult.FaceEmbedding),
            };

            // Save to Neon (PostgreSQL)
            await _userRepository.AddUserAsync(newUser);

            return newUser;
        }
    }
}
