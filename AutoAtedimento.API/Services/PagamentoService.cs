using AutoAtedimento.API.DTO;
using AutoAtedimento.API.Repositories;

namespace AutoAtedimento.API.Services
{
    public class PagamentoService
    {
        private readonly PagamentoRepository _repository;
        private readonly MercadoPagoService _mercadoPagoService;

        public PagamentoService(
            PagamentoRepository repository,
            MercadoPagoService mercadoPagoService)
        {
            _repository = repository;
            _mercadoPagoService = mercadoPagoService;
        }

        public async Task<PagamentoDTO> GerarPix(int sessaoId)
        {
            // 🔥 BUSCA DADOS DO PAGAMENTO
            var pagamento =
                await _repository.ObterDadosPagamento(sessaoId);

            // 🔥 GERA PIX REAL
            var pix =
                await _mercadoPagoService.GerarPix(
                    pagamento.Valor,
                    $"Mesa {pagamento.SessaoId}");

            // 🔥 PREENCHE DTO
            pagamento.TXID = pix.TXID;
            pagamento.QrCode = pix.QrCode;
            pagamento.CopiaECola = pix.CopiaECola;

            // 🔥 SALVA NO BANCO
            await _repository.SalvarPix(
                pagamento.PagamentoId,
                pix);

            return pagamento;
        }

        public async Task ConfirmarPagamento(int pagamentoId)
        {
            await _repository.ConfirmarPagamento(pagamentoId);
        }


        public async Task ProcessarWebhook(
    PagamentoWebhookDTO webhook)
        {
            // 🔥 VALIDAÇÃO
            if (webhook.Data == null)
                return;

            if (string.IsNullOrEmpty(webhook.Data.Id))
                return;

            // 🔥 CONSULTAR MP
            var pagamentoMercadoPago =
                await _mercadoPagoService.ObterPagamento(
                    webhook.Data.Id);

            // 🔥 PAGAMENTO APROVADO?
            if (pagamentoMercadoPago.Status != "approved")
                return;

            // 🔥 BUSCAR NO BANCO
            var pagamentoBanco =
                await _repository.ObterPorMercadoPagoId(
                    webhook.Data.Id);

            if (pagamentoBanco == null)
                return;

            // 🔥 JÁ PAGO?
            if (pagamentoBanco.Status == 2)
                return;

            // 🔥 CONFIRMAR
            await _repository.ConfirmarPagamento(
                pagamentoBanco.PagamentoId);
        }


    }
}