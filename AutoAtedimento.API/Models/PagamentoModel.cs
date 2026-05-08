namespace AutoAtedimento.API.Models
{
    namespace AutoAtedimento.API.Models
    {
        public class PagamentoModel
        {
            public int Pag_Id { get; set; }
            public int Pag_SessaoId { get; set; }
            public decimal Pag_Valor { get; set; }
            public int Pag_Status { get; set; }
            public string? Pag_TXID { get; set; }
            public string? Pag_QrCode { get; set; }
            public string? Pag_CopiaECola { get; set; }
            public DateTime Pag_DataCriacao { get; set; }
            public DateTime? Pag_DataPagamento { get; set; }
        }
    }
}
