using System.Net.Http.Json;
using Biometric.Application.DTO;
using Biometric.Application.Interfaces;

namespace Biometric.Infrastructure.Services
{
    public class FaceAiClient(HttpClient httpClient) : IFaceAiService
    {
        public readonly HttpClient _httpClient = httpClient;

        async Task<FaceAiResponseDto> IFaceAiService.ExtractFaceEmbeddingAsync(
            Stream imageStream,
            string filename
        )
        {
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(imageStream);

            var extension = Path.GetExtension(filename).ToLower();
            string mimeType = extension switch
            {
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "image/jpeg",
            };

            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                mimeType
            );

            // Match the "file" key expected by the API
            content.Add(streamContent, "file", filename);

            var response = await _httpClient.PostAsync("extract-embedding", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Face AI API error: {response.StatusCode}, {errorContent}");
            }

            return await response.Content.ReadFromJsonAsync<FaceAiResponseDto>()
                ?? throw new Exception("Failed to deserialize Face AI response.");
        }
    }
}
