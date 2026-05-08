namespace AutoAtedimento.API.DTO
{
    public class MesaSessaoDTO
    {
        public int SessaoId { get; set; }
        public int MesaId { get; set; }
        public int MesaNumero { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; }
        public int Status { get; set; }
        public decimal Total { get; set; }
    }
}