namespace Shareables.API
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public JwtSettings Jwt { get; set; }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public int Seconds { get; set; }
    }
}
