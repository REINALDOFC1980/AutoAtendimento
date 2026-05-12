namespace AutoAtedimento.API.Models
{
    public class UsuarioModel
    {
        public int Usu_Id { get; set; }

        public string? Usu_Nome { get; set; }

        public string? Usu_Login { get; set; }

        public string? Usu_Senha { get; set; }

        public string? Usu_Perfil { get; set; }

        public bool Usu_Ativo { get; set; }
    }
}