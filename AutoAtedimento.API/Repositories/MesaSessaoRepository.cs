using AutoAtedimento.API.Data;
using AutoAtedimento.API.DTO;
using AutoAtedimento.API.ENUM;
using AutoAtedimento.API.Exceptions;
using Dapper;

namespace AutoAtedimento.API.Repositories
{
    public class MesaSessaoRepository
    {
        private readonly DbSession _db;

        public MesaSessaoRepository(DbSession db)
        {
            _db = db;
        }

        public async Task<int> AbrirSessao(int mesaId)
        {
            using var connection = _db.CreateConnection();

            var mesaExiste = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
                     FROM Mesa
                    WHERE Mes_Id = @MesaId
                       AND Mes_Ativa = 1",
                new { MesaId = mesaId });

            if (mesaExiste == 0)
                throw new NotFoundException("Mesa não encontrada.");

            var sessaoAberta = await connection.ExecuteScalarAsync<int>(
                @"SELECT COUNT(1)
                    FROM MesaSessao
                    WHERE Ses_MesaId = @MesaId
                      AND Ses_Status = 1",
                new { MesaId = mesaId });

            if (sessaoAberta > 0)
                throw new BusinessException("Mesa já possui sessão aberta.");

            var sql = @"
                INSERT INTO MesaSessao
                (
                    Ses_MesaId,
                    Ses_Status
                )
                VALUES
                (
                    @MesaId,
                    @Status
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                MesaId = mesaId,
                Status = (int)StatusSessao.Aberta
            });
        }

        public async Task<FecharSessaoDTO> FecharSessao(int sessaoId)
        {
            using var connection = _db.CreateConnection();

            var sessao = await connection.QueryFirstOrDefaultAsync<dynamic>(
                @"
                    SELECT
                        Ses_Id,
                        Ses_Status
                    FROM MesaSessao
                    WHERE Ses_Id = @SessaoId
                ",
                new { SessaoId = sessaoId });

            if (sessao == null)
                throw new NotFoundException("Sessão não encontrada.");

            if (sessao.Ses_Status == 2)
                throw new BusinessException("Sessão já encerrada.");

            // 🔥 PEDIDOS PENDENTES
            var pedidosPendentes = await connection.ExecuteScalarAsync<int>(
                @"
                    SELECT COUNT(1)
                    FROM Pedido
                    WHERE Ped_SessaoId = @SessaoId
                    AND Ped_Status IN (1,2,3)
                ",
                new { SessaoId = sessaoId });

            if (pedidosPendentes > 0)
                throw new BusinessException(
                    "Existem pedidos pendentes para esta sessão.");

            // 🔥 TOTAL CONSUMIDO
            var total = await connection.ExecuteScalarAsync<decimal>(
                @"
                    SELECT ISNULL(SUM(
                        i.It_Quantidade * i.It_PrecoUnitario
                    ),0)
                    FROM PedidoItem i
                    INNER JOIN Pedido p
                        ON p.Ped_Id = i.It_PedidoId
                    WHERE p.Ped_SessaoId = @SessaoId
                    AND p.Ped_Status = 4
                ",
                new { SessaoId = sessaoId });

            // 🔥 FECHAR SESSÃO
            await connection.ExecuteAsync(
                @"
                    UPDATE MesaSessao
                    SET
                        Ses_Status = 2,
                        Ses_DataFechamento = GETDATE()
                    WHERE Ses_Id = @SessaoId
                ",
                new { SessaoId = sessaoId });

            return new FecharSessaoDTO
            {
                SessaoId = sessaoId,
                Total = total,
                DataFechamento = DateTime.Now
            };
        }
    }
}