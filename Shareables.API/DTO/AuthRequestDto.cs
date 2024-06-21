namespace Shareables.API.DTO
{
    public class AuthRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; }
    }
}
