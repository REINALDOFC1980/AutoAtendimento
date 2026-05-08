using AutoAtedimento.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAtedimento.API.Controllers
{
    [ApiController]
    [Route("api/mesa-sessao")]
    public class MesaSessaoController : ControllerBase
    {
        private readonly MesaSessaoService _service;

        public MesaSessaoController(MesaSessaoService service)
        {
            _service = service;
        }

        [HttpPost("{mesaId}/abrir")]
        public async Task<IActionResult> AbrirSessao(int mesaId)
        {
            var sessaoId =
                await _service.AbrirSessao(mesaId);

            return Ok(new
            {
                sucesso = true,
                sessaoId
            });
        }

        [HttpPut("{sessaoId}/fechar")]
        public async Task<IActionResult> FecharSessao(int sessaoId)
        {
            var result =
                await _service.FecharSessao(sessaoId);

            return Ok(new
            {
                sucesso = true,
                dados = result
            });
        }
    }
}