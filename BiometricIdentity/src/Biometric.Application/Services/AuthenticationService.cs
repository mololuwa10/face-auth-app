using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biometric.Application.Interfaces;

namespace Biometric.Application.Services
{
    public class AuthenticationService(
        IFaceAiService aiService,
        IUserRepository userRepository,
        IJwtProvider jwtProvider
    )
    {
        public async Task<string> LoginWithFaceAsync(Stream imageStream, string filename)
        {
            var _aiResult = await aiService.ExtractFaceEmbeddingAsync(imageStream, filename);

            if (!_aiResult.Success || _aiResult.FaceEmbedding == null)
                throw new Exception("Could not extract facial features from the image.");

            var user =
                await userRepository.FindNearestMatchAsync(_aiResult.FaceEmbedding, 0.9)
                ?? throw new Exception("Face not recognized. Access denied.");

            // Generate the token
            return jwtProvider.GenerateToken(user);
        }
    }
}
