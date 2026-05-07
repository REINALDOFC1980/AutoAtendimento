namespace AutoAtedimento.API.DTO
{
    public class ProdutoDTO
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public decimal Preco { get; set; }

        public int CategoriaId { get; set; }

        public string Categoria { get; set; } = string.Empty;

        public bool Ativo { get; set; }

        public string? Imagem { get; set; }
    }
}