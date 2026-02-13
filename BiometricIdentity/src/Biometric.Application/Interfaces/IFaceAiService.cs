using Biometric.Application.DTO;

namespace Biometric.Application.Interfaces
{
    public interface IFaceAiService
    {
        Task<FaceAiResponseDto> ExtractFaceEmbeddingAsync(Stream imageStream, string filename);
    }
}
