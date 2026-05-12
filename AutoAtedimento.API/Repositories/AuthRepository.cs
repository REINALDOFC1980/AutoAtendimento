using AutoAtedimento.API.Data;
using AutoAtedimento.API.Models;
using Dapper;

namespace AutoAtedimento.API.Repositories
{
    public class AuthRepository
    {
        private readonly DbSession _db;

        public AuthRepository(DbSession db)
        {
            _db = db;
        }

        public async Task<UsuarioModel?> ObterUsuario(string login)
        {
            using var connection = _db.CreateConnection();

            var sql = @"
                SELECT
                    Usu_Id,
                    Usu_Nome,
                    Usu_Login,
                    Usu_Senha,
                    Usu_Perfil,
                    Usu_Ativo
                FROM Usuario
                WHERE Usu_Login = @Login
                AND Usu_Ativo = 1
            ";

            return await connection.QueryFirstOrDefaultAsync<UsuarioModel>(
                sql,
                new
                {
                    Login = login
                });
        }

        public async Task SalvarRefreshToken(int usuarioId, string refreshToken)
                {
            using var connection = _db.CreateConnection();

            var sql = @"
                INSERT INTO RefreshToken
                (
                    Ref_UsuarioId,
                    Ref_Token,
                    Ref_Expiracao
                )
                VALUES
                (
                    @UsuarioId,
                    @Token,
                    @Expiracao
                )
            ";

            await connection.ExecuteAsync(
                sql,
                new
                {
                    UsuarioId = usuarioId,
                    Token = refreshToken,
                    Expiracao = DateTime.Now.AddDays(7)
                });
        }

        public async Task<dynamic?> ObterRefreshToken(string refreshToken)
        {
           using var connection = _db.CreateConnection();

            var sql = @"
            SELECT
                r.Ref_Id,
                r.Ref_UsuarioId,
                r.Ref_Token,
                r.Ref_Expiracao,
                r.Ref_Revogado,

                u.Usu_Id,
                u.Usu_Nome,
                u.Usu_Login,
                u.Usu_Perfil,
                u.Usu_Ativo
            FROM RefreshToken r
            INNER JOIN Usuario u
                ON u.Usu_Id = r.Ref_UsuarioId
            WHERE r.Ref_Token = @Token
            ";

            return await connection.QueryFirstOrDefaultAsync(sql,
            new
            {
                Token = refreshToken
            });
        }

        public async Task RevogarRefreshToken(string refreshToken)
        {
           using var connection = _db.CreateConnection();

            await connection.ExecuteAsync(
                @"
                    UPDATE RefreshToken
                    SET Ref_Revogado = 1
                    WHERE Ref_Token = @Token
                    ",
                new
                {
                    Token = refreshToken
                });
            }
        }
}