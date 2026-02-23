using Biometric.Application.Interfaces;
using Biometric.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Biometric.Application.Services
{
    public class AuthenticationService(
        IFaceAiService aiService,
        IUserRepository userRepository,
        IJwtProvider jwtProvider,
        ILogger<AuthenticationService> logger
    )
    {
        public async Task<(string token, User user)> LoginWithFaceAsync(
            Stream imageStream,
            string filename
        )
        {
            var _aiResult = await aiService.ExtractFaceEmbeddingAsync(imageStream, filename);

            if (!_aiResult.Success || _aiResult.FaceEmbedding == null)
                throw new Exception("Could not extract facial features from the image.");

            // Get the best match and the ACTUAL score
            var (user, similarity) = await userRepository.FindBestMatchWithScoreAsync(
                _aiResult.FaceEmbedding
            );

            // TIGHTEN THE GATE: Change 0.9 to 0.95 or 0.97
            double securityThreshold = 0.9;

            // LOG THE ATTEMPT (This stays hidden from the user)
            logger.LogInformation(
                "Login Attempt: {File} | Best Match: {Email} | Score: {Score:P2}",
                filename,
                user?.Email ?? "None",
                similarity
            );

            if (user == null || similarity < securityThreshold)
            {
                logger.LogWarning(
                    "Access Denied: Score {Score:P2} below threshold {Threshold}",
                    similarity,
                    securityThreshold
                );
                throw new Exception("Face not recognized.");
            }

            return (jwtProvider.GenerateToken(user), user);
        }
    }
}
