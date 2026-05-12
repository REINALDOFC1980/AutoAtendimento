using AutoAtedimento.API.Exceptions;
using AutoAtedimento.API.Models;
using AutoAtedimento.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoAtedimento.API.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _service;

        public ProdutoController(ProdutoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var produtos = await _service.Listar();

            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var produto = await _service.ObterPorId(id);

            return Ok(produto);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] ProdutoModel model)
        {
            var id = await _service.Criar(model);

            return Ok(new
            {
                sucesso = true,
                produtoId = id
            });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] ProdutoModel model)
        {
            await _service.Atualizar(id, model);

            return Ok(new
            {
                sucesso = true,
                mensagem = "Produto atualizado com sucesso."
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Inativar(int id)
        {
            await _service.Inativar(id);

            return Ok(new
            {
                sucesso = true,
                mensagem = "Produto inativado com sucesso."
            });
        }

        [HttpPost("{id}/imagem")]
        public async Task<IActionResult> UploadImagem(int id, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new BusinessException("Arquivo não enviado.");

            var extensoesPermitidas = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            var extensao = Path.GetExtension(arquivo.FileName).ToLower();

            if (!extensoesPermitidas.Contains(extensao))
                throw new BusinessException("Formato de imagem inválido.");

            var nomeArquivo =
                $"{Guid.NewGuid()}{extensao}";

            var caminhoPasta =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "produtos"
                );

            if (!Directory.Exists(caminhoPasta))
                Directory.CreateDirectory(caminhoPasta);

            var caminhoArquivo =
                Path.Combine(caminhoPasta, nomeArquivo);

            using var stream =
                new FileStream(caminhoArquivo, FileMode.Create);

            await arquivo.CopyToAsync(stream);

            await _service.AtualizarImagem(id, nomeArquivo);

            return Ok(new
            {
                sucesso = true,
                imagem = nomeArquivo
            });
        }
    }
}