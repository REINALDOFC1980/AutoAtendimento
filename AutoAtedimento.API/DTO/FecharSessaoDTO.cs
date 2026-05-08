namespace AutoAtedimento.API.DTO
{
    public class FecharSessaoDTO
    {
        public int SessaoId { get; set; }

        public decimal Total { get; set; }

        public DateTime DataFechamento { get; set; }
    }
}