namespace GestionaT.Domain.ValueObjects
{
    public class OAuthUserInfoResult
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Provider { get; set; }
        public Picture Picture { get; set; } = new();
    }

    public class Picture
    {
        public PictureData Data { get; set; } = new();
    }

    public class PictureData
    {
        public string Url { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsSilhouette { get; set; }
    }
}
