namespace AutoAtedimento.API.DTO
{
    public class LoginResponseDTO
    {
        public string? Token { get; set; }

        public DateTime Expiracao { get; set; }
    }
}