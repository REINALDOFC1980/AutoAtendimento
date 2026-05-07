using AutoAtedimento.API.Data;
using AutoAtedimento.API.DTO;
using AutoAtedimento.API.DTO.AutoAtedimento.API.DTO;
using AutoAtedimento.API.Exceptions;
using AutoAtedimento.API.Models;
using Dapper;

namespace AutoAtedimento.API.Repositories
{
    public class CategoriaRepository
    {
        private readonly DbSession _db;

        public CategoriaRepository(DbSession db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CategoriaDTO>> Listar()
        {
            using var connection = _db.CreateConnection();

            var sql = @"
                SELECT
                    Cat_Id AS Id,
                    Cat_Nome AS Nome,
                    Cat_Ativo AS Ativo
                FROM Categoria
                WHERE Cat_Ativo = 1
                ORDER BY Cat_Nome
            ";

            return await connection.QueryAsync<CategoriaDTO>(sql);
        }

        public async Task<CategoriaDTO> ObterPorId(int id)
        {
            using var connection = _db.CreateConnection();

            var sql = @"
                SELECT
                    Cat_Id AS Id,
                    Cat_Nome AS Nome,
                    Cat_Ativo AS Ativo
                FROM Categoria
                WHERE Cat_Id = @Id
            ";

            var categoria = await connection.QueryFirstOrDefaultAsync<CategoriaDTO>(
                sql,
                new { Id = id });

            if (categoria == null)
                throw new NotFoundException("Categoria não encontrada.");

            return categoria;
        }

        public async Task<int> Criar(CategoriaModel model)
        {
            using var connection = _db.CreateConnection();

            var sql = @"
                INSERT INTO Categoria
                (
                    Cat_Nome,
                    Cat_Ativo
                )
                VALUES
                (
                    @Cat_Nome,
                    1
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await connection.ExecuteScalarAsync<int>(sql, model);
        }

        public async Task Atualizar(int id, CategoriaModel model)
        {
            using var connection = _db.CreateConnection();

            var existe = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
                  FROM Categoria
                  WHERE Cat_Id = @Id",
                new { Id = id });

            if (existe == 0)
                throw new NotFoundException("Categoria não encontrada.");

            var sql = @"
                UPDATE Categoria
                SET Cat_Nome = @Cat_Nome
                WHERE Cat_Id = @Id
            ";

            await connection.ExecuteAsync(sql, new
            {
                Id = id,
                model.Cat_Nome
            });
        }

        public async Task Inativar(int id)
        {
            using var connection = _db.CreateConnection();

            var existe = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
                  FROM Categoria
                  WHERE Cat_Id = @Id",
                new { Id = id });

            if (existe == 0)
                throw new NotFoundException("Categoria não encontrada.");

            var sql = @"
                UPDATE Categoria
                SET Cat_Ativo = 0
                WHERE Cat_Id = @Id
            ";

            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }
}