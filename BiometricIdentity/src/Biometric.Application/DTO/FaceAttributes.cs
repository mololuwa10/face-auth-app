namespace Biometric.Application.DTO
{
    public class FaceAttributes
    {
        public double Age { get; set; }
        public string DominantGender { get; set; } = string.Empty;
        public string DominantRace { get; set; } = string.Empty;
        public string DominantEmotion { get; set; } = string.Empty;
    }
}
