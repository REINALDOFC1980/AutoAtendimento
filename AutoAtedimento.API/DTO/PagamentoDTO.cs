namespace AutoAtedimento.API.DTO
{
    namespace AutoAtedimento.API.DTO
    {
        public class PagamentoDTO
        {
            public int PagamentoId { get; set; }
            public int SessaoId { get; set; }
            public decimal Valor { get; set; }
            public int Status { get; set; }
            public string? TXID { get; set; }
            public string? QrCode { get; set; }
            public string? CopiaECola { get; set; }
            public DateTime DataCriacao { get; set; }
            public DateTime? DataPagamento { get; set; }
        }
    }
}
