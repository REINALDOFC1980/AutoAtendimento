using AutoAtedimento.API.DTO;
using AutoAtedimento.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoAtedimento.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _service;

        public AuthController(AuthService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _service.Login(dto);

            return Ok(result);
        }
    }
}