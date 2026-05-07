using AutoAtedimento.API.Data;
using AutoAtedimento.API.DTO;
using AutoAtedimento.API.Exceptions;
using AutoAtedimento.API.Models;
using Dapper;

namespace AutoAtedimento.API.Repositories
{
    public class ProdutoRepository
    {
        private readonly DbSession _db;

        public ProdutoRepository(DbSession db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ProdutoDTO>> Listar()
        {
            using var connection = _db.CreateConnection();

            var sql = @"
                        SELECT
                            p.Pro_Id AS Id,
                            p.Pro_Nome AS Nome,
                            p.Pro_Descricao AS Descricao,
                            p.Pro_Preco AS Preco,
                            p.Pro_Ativo AS Ativo,
                            c.Cat_Id AS CategoriaId,
                            c.Cat_Nome AS Categoria,
                            p.Pro_Imagem AS Imagem
                        FROM Produto p
                        INNER JOIN Categoria c
                            ON c.Cat_Id = p.Pro_CategoriaId
                        WHERE p.Pro_Ativo = 1
                        ORDER BY c.Cat_Ordem, p.Pro_Nome
                    ";

            return await connection.QueryAsync<ProdutoDTO>(sql);
        }

        public async Task<ProdutoDTO> ObterPorId(int id)
        {
            using var connection = _db.CreateConnection();

            var sql = @"
                    SELECT
                        p.Pro_Id AS Id,
                        p.Pro_Nome AS Nome,
                        p.Pro_Descricao AS Descricao,
                        p.Pro_Preco AS Preco,
                        p.Pro_Ativo AS Ativo,
                        c.Cat_Id AS CategoriaId,
                        c.Cat_Nome AS Categoria,
                        p.Pro_Imagem AS Imagem
                    FROM Produto p
                    INNER JOIN Categoria c
                        ON c.Cat_Id = p.Pro_CategoriaId
                    WHERE p.Pro_Id = @Id
                ";

            var produto = await connection.QueryFirstOrDefaultAsync<ProdutoDTO>(
                sql,
                new { Id = id });

            if (produto == null)
                throw new NotFoundException("Produto não encontrado.");

            return produto;
        }

        public async Task<int> Criar(ProdutoModel model)
        {
            using var connection = _db.CreateConnection();


            var categoriaExiste = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
                    FROM Categoria
                    WHERE Cat_Id = @CategoriaId",
                new
                {
                    CategoriaId = model.Pro_CategoriaId
                });

            if (categoriaExiste == 0)
                throw new BusinessException("Categoria não encontrada.");


            var sql = @"
                INSERT INTO Produto
                (
                    Pro_Nome,
                    Pro_Descricao,
                    Pro_Preco,
                    Pro_Ativo,
                    Pro_CategoriaId
                )
                VALUES
                (
                    @Pro_Nome,
                    @Pro_Descricao,
                    @Pro_Preco,
                    1,
                    @Pro_CategoriaId
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await connection.ExecuteScalarAsync<int>(sql, model);
        }

        public async Task Atualizar(int id, ProdutoModel model)
        {
            using var connection = _db.CreateConnection();

            var produtoExiste = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
                      FROM Produto
                      WHERE Pro_Id = @Id",
                new { Id = id });

            if (produtoExiste == 0)
                throw new NotFoundException("Produto não encontrado.");

            var sql = @"
                        UPDATE Produto
                        SET
                            Pro_Nome = @Pro_Nome,
                            Pro_Descricao = @Pro_Descricao,
                            Pro_Preco = @Pro_Preco
                        WHERE Pro_Id = @Id
                    ";

            await connection.ExecuteAsync(sql, new
            {
                Id = id,
                model.Pro_Nome,
                model.Pro_Descricao,
                model.Pro_Preco
            });
        }

        public async Task Inativar(int id)
        {
            using var connection = _db.CreateConnection();

            var produtoExiste = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
                          FROM Produto
                          WHERE Pro_Id = @Id",
                new { Id = id });

            if (produtoExiste == 0)
                throw new NotFoundException("Produto não encontrado.");

            var sql = @"
                        UPDATE Produto
                        SET Pro_Ativo = 0
                        WHERE Pro_Id = @Id
                    ";

            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task AtualizarImagem(int id, string nomeArquivo)
        {
            using var connection = _db.CreateConnection();

            var produtoExiste = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
                          FROM Produto
                          WHERE Pro_Id = @Id",
                new { Id = id });

            if (produtoExiste == 0)
                throw new NotFoundException("Produto não encontrado.");

            var sql = @"
                        UPDATE Produto
                        SET Pro_Imagem = @Imagem
                        WHERE Pro_Id = @Id
                    ";

            await connection.ExecuteAsync(sql, new
            {
                Id = id,
                Imagem = nomeArquivo
            });
        }
    }
}