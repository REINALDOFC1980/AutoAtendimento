using AutoAtedimento.API.Data;
using AutoAtedimento.API.DTO;
using AutoAtedimento.API.DTO.AutoAtedimento.API.DTO;
using AutoAtedimento.API.ENUM;
using AutoAtedimento.API.Exceptions;
using AutoAtedimento.API.Models;
using Dapper;
using System.Data;


namespace AutoAtedimento.API.Repositories
{
    public class PedidoRepository
    {
        private readonly DbSession _db;

        public PedidoRepository(DbSession db)
        {
            _db = db;
        }

        public async Task<int> CriarPedido(PedidoModel pedido)
        {
            using var connection = _db.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                var mesaExiste = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) 
                          FROM Mesa 
                          WHERE Mes_Id = @MesaId",
                    new { MesaId = pedido.Ped_MesaId },
                    transaction
                );

                if (mesaExiste == 0)
                    throw new BusinessException("Mesa não encontrada.");

                var sqlPedido = @"
                        INSERT INTO Pedido
                        (
                            Ped_MesaId,
                            Ped_Status,
                            Ped_DataHora,
                            Ped_Observacao
                        )
                        VALUES
                        (
                            @MesaId,
                            @Status,
                            GETDATE(),
                            @Observacao
                        );

                        SELECT CAST(SCOPE_IDENTITY() AS INT);
                        ";

                var pedidoId = await connection.ExecuteScalarAsync<int>(
                    sqlPedido,
                    new
                    {
                        MesaId = pedido.Ped_MesaId,
                        Status = (int)StatusPedido.Recebido,
                        Observacao = pedido.Ped_Observacao
                    },
                    transaction
                );

                foreach (var item in pedido.Itens)
                {
                    var produtoExiste = await connection.ExecuteScalarAsync<int>(
                        @"SELECT COUNT(1)
                              FROM Produto
                              WHERE Pro_Id = @ProdutoId",
                                    new
                        {
                            ProdutoId = item.It_ProdutoId
                        },
                        transaction
                    );

                    if (produtoExiste == 0)
                        throw new BusinessException($"Produto {item.It_ProdutoId} não encontrado.");

                    var sqlItem = @"
                            INSERT INTO PedidoItem
                            (
                                It_PedidoId,
                                It_ProdutoId,
                                It_Quantidade,
                                It_PrecoUnitario
                            )
                            SELECT
                                @PedidoId,
                                Pro_Id,
                                @Quantidade,
                                Pro_Preco
                            FROM Produto
                            WHERE Pro_Id = @ProdutoId
                        ";

                    await connection.ExecuteAsync(
                        sqlItem,
                        new
                        {
                            PedidoId = pedidoId,
                            ProdutoId = item.It_ProdutoId,
                            Quantidade = item.It_Quantidade
                        },
                        transaction
                    );
                }

                transaction.Commit();

                return pedidoId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<IEnumerable<PedidoCozinhaDTO>> ListarPedidosCozinha()
        {
            using var connection = _db.CreateConnection();

            var sql = @"
                        SELECT 
                            p.Ped_Id AS PedidoId,
                            m.Mes_Numero AS Mesa,
                            p.Ped_DataHora AS DataHora,
                            p.Ped_Status AS StatusId,
                            pr.Pro_Nome AS Produto,
                            i.It_Quantidade AS Quantidade
                        FROM Pedido p
                        INNER JOIN Mesa m 
                            ON m.Mes_Id = p.Ped_MesaId
                        INNER JOIN PedidoItem i 
                            ON i.It_PedidoId = p.Ped_Id
                        INNER JOIN Produto pr 
                            ON pr.Pro_Id = i.It_ProdutoId
                        WHERE p.Ped_Status IN (@Recebido, @EmPreparo)
                        ORDER BY p.Ped_DataHora ASC
                    ";

            var lookup = new Dictionary<int, PedidoCozinhaDTO>();

            var result = await connection.QueryAsync<PedidoCozinhaDTO, PedidoItemDTO, PedidoCozinhaDTO>(
                sql,
                (pedido, item) =>
                {
                    if (!lookup.TryGetValue(pedido.PedidoId, out var pedidoExistente))
                    {
                        pedidoExistente = pedido;

                        pedidoExistente.Itens = new List<PedidoItemDTO>();

                        // 🔥 Converter enum para texto amigável
                        pedidoExistente.Status =
                            ((StatusPedido)pedido.StatusId).ToString();

                        lookup.Add(pedidoExistente.PedidoId, pedidoExistente);
                    }

                    pedidoExistente.Itens.Add(item);

                    return pedidoExistente;
                },
                new
                {
                    Recebido = (int)StatusPedido.Recebido,
                    EmPreparo = (int)StatusPedido.EmPreparo
                },
                splitOn: "Produto"
            );

            return lookup.Values;
        }
        public async Task AtualizarStatus(int pedidoId, StatusPedido status)
        {
            using var connection = _db.CreateConnection();

            var statusAtual = await connection.ExecuteScalarAsync<int?>(
                            @"SELECT Ped_Status
                      FROM Pedido
                      WHERE Ped_Id = @PedidoId",
                  new { PedidoId = pedidoId });

            if (statusAtual == null)
                throw new NotFoundException("Pedido não encontrado.");

            var statusAtualEnum = (StatusPedido)statusAtual.Value;

            if (statusAtualEnum == StatusPedido.Finalizado ||
                statusAtualEnum == StatusPedido.Cancelado)
            {
                throw new BusinessException("Pedido já encerrado.");
            }

            if (status == statusAtualEnum)
            {
                throw new BusinessException("O pedido já está nesse status.");
            }

            if (!TransicaoValida(statusAtualEnum, status))
            {
                throw new BusinessException(
                    $"Não é permitido alterar de {statusAtualEnum} para {status}."
                );
            }

            var sql = @"
                UPDATE Pedido
                SET Ped_Status = @Status
                WHERE Ped_Id = @PedidoId
            ";

            await connection.ExecuteAsync(sql, new
            {
                PedidoId = pedidoId,
                Status = (int)status
            });
        }

        private bool TransicaoValida(StatusPedido atual, StatusPedido novo)
        {
            return atual switch
            {
                StatusPedido.Recebido =>
                    novo == StatusPedido.EmPreparo ||
                    novo == StatusPedido.Cancelado,

                StatusPedido.EmPreparo =>
                    novo == StatusPedido.Pronto ||
                    novo == StatusPedido.Cancelado,

                StatusPedido.Pronto =>
                    novo == StatusPedido.Finalizado,

                _ => false
            };
        }


        public async Task<PedidoDetalheDTO> ObterPedidoPorId(int pedidoId)
        {
            using var connection = _db.CreateConnection();

            var sql = @"
                        SELECT 
                            p.Ped_Id AS PedidoId,
                            m.Mes_Numero AS Mesa,
                            p.Ped_DataHora AS DataHora,
                            p.Ped_Status AS Status,
                            pr.Pro_Nome AS Item_Produto,
                            i.It_Quantidade AS Quantidade,
                            i.It_PrecoUnitario AS Preco
                        FROM Pedido p
                        INNER JOIN Mesa m ON m.Mes_Id = p.Ped_MesaId
                        LEFT JOIN PedidoItem i ON i.It_PedidoId = p.Ped_Id
                        LEFT JOIN Produto pr ON pr.Pro_Id = i.It_ProdutoId
                        WHERE p.Ped_Id = @pedidoId
                            ";

            var lookup = new Dictionary<int, PedidoDetalheDTO>();

            var result = await connection.QueryAsync<PedidoDetalheDTO, PedidoItemDetalheDTO, PedidoDetalheDTO>(
                  sql,
                  (pedido, item) =>
                  {
                      if (!lookup.TryGetValue(pedido.PedidoId, out var pedidoExistente))
                      {
                          pedidoExistente = pedido;
                          pedidoExistente.Itens = new List<PedidoItemDetalheDTO>();
                          lookup.Add(pedidoExistente.PedidoId, pedidoExistente);
                      }

                      if (item != null && !string.IsNullOrEmpty(item.Item_Produto))
                          pedidoExistente.Itens.Add(item);

                      return pedidoExistente;
                  },
                  new { pedidoId },
                  splitOn: "Item_Produto" // 🔥 TEM que bater com alias
              );

            return lookup.Values.FirstOrDefault();
        }
        public async Task FinalizarPedido(int pedidoId)
        {
            using var connection = _db.CreateConnection();

            var statusAtual = await connection.ExecuteScalarAsync<int?>(
                @"SELECT Ped_Status
                          FROM Pedido
                          WHERE Ped_Id = @PedidoId",
                new { PedidoId = pedidoId });

            if (statusAtual == null)
                throw new NotFoundException("Pedido não encontrado.");

            var statusAtualEnum = (StatusPedido)statusAtual.Value;

            if (statusAtualEnum == StatusPedido.Cancelado)
                throw new BusinessException("Pedido cancelado não pode ser finalizado.");

            if (statusAtualEnum == StatusPedido.Finalizado)
                throw new BusinessException("Pedido já foi finalizado.");

            if (statusAtualEnum != StatusPedido.Pronto)
                throw new BusinessException("Pedido ainda não está pronto.");

            var sql = @"
                    UPDATE Pedido
                    SET Ped_Status = @Status
                    WHERE Ped_Id = @PedidoId
                ";

            await connection.ExecuteAsync(sql, new
            {
                PedidoId = pedidoId,
                Status = (int)StatusPedido.Finalizado
            });
        }

        public async Task CancelarPedido(int pedidoId)
        {
            using var connection = _db.CreateConnection();

            var statusAtual = await connection.ExecuteScalarAsync<int?>(
                @"SELECT Ped_Status
                          FROM Pedido
                          WHERE Ped_Id = @PedidoId",
                                new { PedidoId = pedidoId });

            if (statusAtual == null)
                throw new NotFoundException("Pedido não encontrado.");

            var statusAtualEnum = (StatusPedido)statusAtual.Value;

            // 🔥 NÃO PODE CANCELAR FINALIZADO
            if (statusAtualEnum == StatusPedido.Finalizado)
                throw new BusinessException(
                    "Pedido já finalizado não pode ser cancelado."
                );

            // 🔥 NÃO PODE CANCELAR CANCELADO
            if (statusAtualEnum == StatusPedido.Cancelado)
                throw new BusinessException(
                    "Pedido já está cancelado."
                );

                var sql = @"
                    UPDATE Pedido
                    SET Ped_Status = @Status
                    WHERE Ped_Id = @PedidoId
                ";

            await connection.ExecuteAsync(sql, new
            {
                PedidoId = pedidoId,
                Status = (int)StatusPedido.Cancelado
            });
        }

        public async Task<IEnumerable<PedidoDetalheDTO>> ObterPedidosPorMesa(int mesaId)
        {
            using var connection = _db.CreateConnection();

            var sql = @"
                        SELECT 
                            p.Ped_Id AS PedidoId,
                            m.Mes_Numero AS Mesa,
                            p.Ped_DataHora AS DataHora,
                            p.Ped_Status AS Status,
                            pr.Pro_Nome AS Item_Produto,
                            i.It_Quantidade AS Quantidade,
                            i.It_PrecoUnitario AS Preco
                        FROM Pedido p
                        INNER JOIN Mesa m ON m.Mes_Id = p.Ped_MesaId
                        LEFT JOIN PedidoItem i ON i.It_PedidoId = p.Ped_Id
                        LEFT JOIN Produto pr ON pr.Pro_Id = i.It_ProdutoId
                        WHERE p.Ped_MesaId = @mesaId
                        ORDER BY p.Ped_DataHora DESC
    ";

            var lookup = new Dictionary<int, PedidoDetalheDTO>();

            var result = await connection.QueryAsync<PedidoDetalheDTO, PedidoItemDetalheDTO, PedidoDetalheDTO>(
                sql,
                (pedido, item) =>
                {
                    if (!lookup.TryGetValue(pedido.PedidoId, out var pedidoExistente))
                    {
                        pedidoExistente = pedido;
                        pedidoExistente.Itens = new List<PedidoItemDetalheDTO>();
                        lookup.Add(pedidoExistente.PedidoId, pedidoExistente);
                    }

                    if (item != null && !string.IsNullOrEmpty(item.Item_Produto))
                        pedidoExistente.Itens.Add(item);

                    return pedidoExistente;
                },
                new { mesaId },
                splitOn: "Item_Produto"
            );

            return lookup.Values;
        }



        public async Task<PedidoTotalDTO> ObterTotalPedido(int pedidoId)
        {
            using var connection = _db.CreateConnection();

            var pedidoExiste = await connection.ExecuteScalarAsync<int>(
                        @"SELECT COUNT(1)
                            FROM Pedido
                           WHERE Ped_Id = @PedidoId",
                            new { PedidoId = pedidoId });

                        if (pedidoExiste == 0)
                            throw new NotFoundException("Pedido não encontrado.");

                        var sql = @"
                                SELECT
                                    It_PedidoId AS PedidoId,
                                    SUM(It_Quantidade * It_PrecoUnitario) AS Total
                                FROM PedidoItem
                                WHERE It_PedidoId = @PedidoId
                                GROUP BY It_PedidoId
                            ";

            var total = await connection.QueryFirstOrDefaultAsync<PedidoTotalDTO>(
                sql,
                new { PedidoId = pedidoId });

            return total ?? new PedidoTotalDTO
            {
                PedidoId = pedidoId,
                Total = 0
            };
        }
    }
}
