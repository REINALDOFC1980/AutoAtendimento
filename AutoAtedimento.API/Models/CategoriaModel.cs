namespace AutoAtedimento.API.Models
{
    public class CategoriaModel
    {
        public int Cat_Id { get; set; }

        public string Cat_Nome { get; set; } = string.Empty;

        public bool Cat_Ativo { get; set; } = true;
    }
}