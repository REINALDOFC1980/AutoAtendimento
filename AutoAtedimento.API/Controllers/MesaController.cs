using AutoAtedimento.API.Models;
using AutoAtedimento.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace AutoAtedimento.API.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [ApiController]
    [Route("api/[controller]")]
    public class MesaController : ControllerBase
    {
        private readonly MesaService _service;

        public MesaController(MesaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] MesaModel model)
        {
            var id = await _service.Criar(model);

            return Ok(new
            {
                sucesso = true,
                mesaId = id
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

        [HttpGet("{id}/qrcode")]
        public IActionResult GerarQRCode(int id)
        {
            var url =
                $"https://localhost:7253/m/{id}";

            using var qrGenerator = new QRCodeGenerator();

            using var qrCodeData =
                qrGenerator.CreateQrCode(
                    url,
                    QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);

            byte[] qrCodeImage =
                qrCode.GetGraphic(20);

            return File(qrCodeImage, "image/png");
        }
    }
}