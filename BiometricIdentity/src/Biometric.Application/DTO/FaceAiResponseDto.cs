using System.Text.Json.Serialization;

namespace Biometric.Application.DTO
{
    public class FaceAiResponseDto
    {
        public bool Success { get; set; }

        [JsonPropertyName("embedding")]
        public float[]? FaceEmbedding { get; set; } = [];
        public double FaceConfidence { get; set; }
        public FaceAttributes Attributes { get; set; } = new();
    }
}
