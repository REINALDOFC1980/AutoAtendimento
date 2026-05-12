using AutoAtedimento.API.DTO;
using AutoAtedimento.API.Exceptions;
using AutoAtedimento.API.Repositories;

namespace AutoAtedimento.API.Services
{
    public class AuthService
    {
        private readonly AuthRepository _repository;
        private readonly JwtService _jwtService;

        public AuthService(
            AuthRepository repository,
            JwtService jwtService)
        {
            _repository = repository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDTO> Login(LoginDTO dto)
        {
            var usuario = await _repository.Login(
                    dto.Login!,
                    dto.Senha!);

            if (usuario == null)
                throw new BusinessException(
                    "Usuário ou senha inválidos.");

            var token = _jwtService.GerarToken(usuario);

            return new LoginResponseDTO
            {
                Token = token,
                Expiracao = DateTime.Now.AddHours(8)
            };
        }
    }
}