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

        public async Task<LoginResponseDTO> Login( LoginDTO dto)
        {
            var usuario = await _repository.ObterUsuario(dto.Login!);

            if (usuario == null)
                throw new BusinessException(
                    "Usuário ou senha inválidos.");

            // 🔥 VALIDAR HASH
            var senhaValida = BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.Usu_Senha);

            if (!senhaValida)
                throw new BusinessException("Usuário ou senha inválidos.");

            var token = _jwtService.GerarToken(usuario);
            var refreshToken = _jwtService.GerarRefreshToken();
            await _repository.SalvarRefreshToken(usuario.Usu_Id,refreshToken);

            return new LoginResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                Expiracao = DateTime.Now.AddMinutes(15)
            };
        }
        public async Task<LoginResponseDTO> RefreshToken(string refreshToken)
        {
            var tokenBanco =
                await _repository.ObterRefreshToken(refreshToken);

            if (tokenBanco == null)
                throw new BusinessException("Refresh token inválido.");

            if (tokenBanco.Ref_Revogado)
                throw new BusinessException("Refresh token revogado.");

            if (tokenBanco.Ref_Expiracao < DateTime.Now)
                throw new BusinessException("Refresh token expirado.");

            var usuario =
                new Models.UsuarioModel
                {
                    Usu_Id = tokenBanco.Usu_Id,
                    Usu_Nome = tokenBanco.Usu_Nome,
                    Usu_Login = tokenBanco.Usu_Login,
                    Usu_Perfil = tokenBanco.Usu_Perfil
                };

            var novoJwt = _jwtService.GerarToken(usuario);

            var novoRefresh = _jwtService.GerarRefreshToken();

            // 🔥 REVOGAR ANTIGO
            await _repository.RevogarRefreshToken(refreshToken);

            // 🔥 SALVAR NOVO
            await _repository.SalvarRefreshToken(usuario.Usu_Id,novoRefresh);

            return new LoginResponseDTO
            {
                Token = novoJwt,
                RefreshToken = novoRefresh,
                Expiracao = DateTime.Now.AddMinutes(15)
            };
        }
    }
}