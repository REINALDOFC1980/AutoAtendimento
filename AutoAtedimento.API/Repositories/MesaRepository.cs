using AutoAtedimento.API.Data;
using AutoAtedimento.API.DTO;
using AutoAtedimento.API.Exceptions;
using AutoAtedimento.API.Models;
using Dapper;

namespace AutoAtedimento.API.Repositories
{
    public class MesaRepository
    {
        private readonly DbSession _db;

        public MesaRepository(DbSession db)
        {
            _db = db;
        }

        public async Task<IEnumerable<MesaDTO>> Listar()
        {
            using var connection = _db.CreateConnection();

            var sql = @"
                SELECT
                    Mes_Id AS Id,
                    Mes_Numero AS Numero,
                    Mes_Descricao AS Descricao,
                    Mes_Ativa AS Ativa
                FROM Mesa
                WHERE Mes_Ativa = 1
                ORDER BY Mes_Numero
            ";

            return await connection.QueryAsync<MesaDTO>(sql);
        }

        public async Task<int> Criar(MesaModel model)
        {
            using var connection = _db.CreateConnection();

            var existe = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
                  FROM Mesa
                  WHERE Mes_Numero = @Numero",
                new
                {
                    Numero = model.Mes_Numero
                });

            if (existe > 0)
                throw new BusinessException("Mesa já cadastrada.");

            var sql = @"
                INSERT INTO Mesa
                (
                    Mes_Numero,
                    Mes_Descricao,
                    Mes_Ativa
                )
                VALUES
                (
                    @Mes_Numero,
                    @Mes_Descricao,
                    1
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await connection.ExecuteScalarAsync<int>(sql, model);
        }

        public async Task Inativar(int id)
        {
            using var connection = _db.CreateConnection();

            var sql = @"
                UPDATE Mesa
                SET Mes_Ativa = 0
                WHERE Mes_Id = @Id
            ";

            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}