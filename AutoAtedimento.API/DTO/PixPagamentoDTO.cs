namespace AutoAtedimento.API.DTO
{
    public class PixPagamentoDTO
    {
        public string TXID { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
        public string CopiaECola { get; set; } = string.Empty;
        public string MercadoPagoId { get; set; } = string.Empty;
    }
}