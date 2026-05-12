namespace AutoAtedimento.API.DTO
{
    public class LoginResponseDTO
    {
        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime Expiracao { get; set; }
    }
}