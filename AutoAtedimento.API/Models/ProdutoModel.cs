namespace AutoAtedimento.API.Models
{
    public class ProdutoModel
    {
        public int Pro_Id { get; set; }

        public int Pro_CategoriaId { get; set; }

        public string Pro_Nome { get; set; } = string.Empty;

        public string? Pro_Descricao { get; set; }

        public decimal Pro_Preco { get; set; }

        public bool Pro_Ativo { get; set; } = true;
    }
}