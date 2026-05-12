using AutoAtedimento.API.Models;
using AutoAtedimento.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAtedimento.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly PedidoService _service;

        public PedidoController(PedidoService service)
        {
            _service = service;
        }

        [HttpPost("criar")]
        public async Task<IActionResult> CriarPedido([FromBody] PedidoModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pedidoId = await _service.CriarPedido(model);

            return Ok(new { sucesso = true, pedidoId });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("cozinha")]
        public async Task<IActionResult> ListarCozinha()
        {
            var pedidos = await _service.ListarPedidosCozinha();
            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPedido(int id)
        {
            var pedido = await _service.ObterPedidoPorId(id);
            return Ok(pedido);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusModel status)
        {
            await _service.AtualizarStatus(id, status.Status);

            return Ok(new
            {
                sucesso = true,
                mensagem = "Status atualizado com sucesso"
            });
        }

        [HttpPut("{id}/finalizar")]
        public async Task<IActionResult> Finalizar(int id)
        {
            await _service.FinalizarPedido(id);
            return Ok(new
            {
                sucesso = true,
                mensagem = "Pedido finalizado com sucesso"
            });
        }

        [HttpPut("{id}/cancelar")]
        public async Task<IActionResult> Cancelar(int id)
        {
            await _service.CancelarPedido(id);
            return Ok(new
            {
                sucesso = true,
                mensagem = "Pedido cancelado com sucesso"
            });
        }


        [HttpGet("{id}/total")]
        public async Task<IActionResult> ObterTotal(int id)
        {
            var total = await _service.ObterTotalPedido(id);

            return Ok(total);
        }   

    }
}