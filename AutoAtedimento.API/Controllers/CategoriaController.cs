using AutoAtedimento.API.Models;
using AutoAtedimento.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAtedimento.API.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly CategoriaService _service;

        public CategoriaController(CategoriaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            return Ok(await _service.ObterPorId(id));
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CategoriaModel model)
        {
            var id = await _service.Criar(model);

            return Ok(new
            {
                sucesso = true,
                categoriaId = id
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(
            int id,
            [FromBody] CategoriaModel model)
        {
            await _service.Atualizar(id, model);

            return Ok(new
            {
                sucesso = true
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Inativar(int id)
        {
            await _service.Inativar(id);

            return Ok(new
            {
                sucesso = true
            });
        }
    }
}