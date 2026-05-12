using AutoAtedimento.API.DTO;
using AutoAtedimento.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAtedimento.API.Controllers
{
    [ApiController]
    [Route("api/pagamento")]
    public class PagamentoController : ControllerBase
    {
        private readonly PagamentoService _service;

        public PagamentoController(PagamentoService service)
        {
            _service = service;
        }

        [HttpPost("{sessaoId}/pix")]
        public async Task<IActionResult> GerarPix(int sessaoId)
        {
            var pagamento =
                await _service.GerarPix(sessaoId);

            return Ok(new
            {
                sucesso = true,
                dados = pagamento
            });
        }

        [HttpPut("{pagamentoId}/confirmar")]
        public async Task<IActionResult> ConfirmarPagamento(int pagamentoId)
        {
            await _service.ConfirmarPagamento(pagamentoId);

            return Ok(new
            {
                sucesso = true,
                mensagem = "Pagamento confirmado com sucesso."
            });
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] PagamentoWebhookDTO webhook)
        {
            await _service.ProcessarWebhook(webhook);

            return Ok();
        }
    }
}