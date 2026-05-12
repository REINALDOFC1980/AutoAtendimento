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

        public async Task<UsuarioModel?> Login(
            string login,
            string senha)
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
                AND Usu_Senha = @Senha
                AND Usu_Ativo = 1
            ";

            return await connection.QueryFirstOrDefaultAsync<UsuarioModel>(
                sql,
                new
                {
                    Login = login,
                    Senha = senha
                });
        }
    }
}