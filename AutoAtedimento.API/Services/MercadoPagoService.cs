using AutoAtedimento.API.DTO;
using MercadoPago.Client.Payment;
using MercadoPago.Resource.Payment;

namespace AutoAtedimento.API.Services
{
    public class MercadoPagoService
    {
        public async Task<PixPagamentoDTO> GerarPix(
            decimal valor,
            string descricao)
        {
            var client = new PaymentClient();

            var request = new PaymentCreateRequest
            {
                TransactionAmount = valor,

                Description = descricao,

                PaymentMethodId = "pix",

                Payer = new PaymentPayerRequest
                {
                    Email = "reinaldofc80@gmail.com"
                },

              
            };

            Payment pagamento = await client.CreateAsync(request);

            return new PixPagamentoDTO
            {
                TXID = pagamento.Id.ToString(),

                QrCode =
                    pagamento.PointOfInteraction
                    .TransactionData
                    .QrCodeBase64,

                CopiaECola =
                    pagamento.PointOfInteraction
                    .TransactionData
                    .QrCode,

                 MercadoPagoId = pagamento.Id.ToString()
            };

        }
        public async Task<Payment> ObterPagamento(string paymentId)
        {
            var client = new PaymentClient();

            return await client.GetAsync(
                long.Parse(paymentId));
        }
    }
}