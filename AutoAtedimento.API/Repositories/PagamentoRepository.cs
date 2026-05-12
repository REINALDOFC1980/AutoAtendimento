using AutoAtedimento.API.Data;
using AutoAtedimento.API.DTO;
using AutoAtedimento.API.DTO.AutoAtedimento.API.DTO;
using AutoAtedimento.API.ENUM;

using AutoAtedimento.API.Exceptions;
using Dapper;

namespace AutoAtedimento.API.Repositories
{
    public class PagamentoRepository
    {
        private readonly DbSession _db;

        public PagamentoRepository(DbSession db)
        {
            _db = db;
        }


        public async Task ConfirmarPagamento(int pagamentoId)
        {
            using var connection = _db.CreateConnection();

            // 🔥 BUSCAR PAGAMENTO
            var pagamento = await connection.QueryFirstOrDefaultAsync<dynamic>(
                @"
                SELECT
                      Pag_Id,
                     Pag_SessaoId,
                     Pag_Status
                FROM Pagamento
                WHERE Pag_Id = @PagamentoId
                ",
                new { PagamentoId = pagamentoId });

            if (pagamento == null)
                throw new NotFoundException("Pagamento não encontrado.");

            if (pagamento.Pag_Status == 2)
                throw new BusinessException("Pagamento já confirmado.");

            // 🔥 VALIDAR PEDIDOS PENDENTES
            var pedidosPendentes =
                await connection.ExecuteScalarAsync<int>(
                    @"
                    SELECT COUNT(1)
                    FROM Pedido
                    WHERE Ped_SessaoId = @SessaoId
                    AND Ped_Status IN (1,2,3)
                    ",
                    new
                    {
                        SessaoId = pagamento.Pag_SessaoId
                    });

            if (pedidosPendentes > 0)
                throw new BusinessException(
                    "Ainda existem pedidos pendentes.");

            // 🔥 CONFIRMAR PAGAMENTO
            await connection.ExecuteAsync(
                @"
                UPDATE Pagamento
                SET
                    Pag_Status = 2,
                    Pag_DataPagamento = GETDATE()
                WHERE Pag_Id = @PagamentoId
                ",
                new { PagamentoId = pagamentoId });

            // 🔥 FECHAR SESSÃO
            await connection.ExecuteAsync(
                @"
                UPDATE MesaSessao
                SET
                    Ses_Status = 2,
                    Ses_DataFechamento = GETDATE()
                WHERE Ses_Id = @SessaoId
                ",
                new
                {
                    SessaoId = pagamento.Pag_SessaoId
                });
        }


        public async Task<PagamentoDTO> ObterDadosPagamento(int sessaoId)
        {
            using var connection = _db.CreateConnection();

            // 🔥 VALIDAR SESSÃO
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

            if (sessao.Ses_Status != 1)
                throw new BusinessException("Sessão encerrada.");

            // 🔥 VALIDAR PAGAMENTO PENDENTE
            var pagamentoExistente =
                await connection.QueryFirstOrDefaultAsync<int?>(
                    @"
                    SELECT Pag_Id
                    FROM Pagamento
                    WHERE Pag_SessaoId = @SessaoId
                    AND Pag_Status = 1
                    ",
                    new { SessaoId = sessaoId });

            if (pagamentoExistente.HasValue)
                throw new BusinessException(
                    "Já existe pagamento pendente.");

            // 🔥 CALCULAR TOTAL
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

            if (total <= 0)
                throw new BusinessException(
                    "Sessão sem pedidos finalizados.");

            // 🔥 CRIAR REGISTRO PAGAMENTO
            var pagamentoId = await connection.ExecuteScalarAsync<int>(
                @"
                INSERT INTO Pagamento
                (
                    Pag_SessaoId,
                    Pag_Valor,
                    Pag_Status
                )
                VALUES
                (
                    @SessaoId,
                    @Valor,
                    1
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);
                ",
                new
                {
                    SessaoId = sessaoId,
                    Valor = total
                });

            return new PagamentoDTO
            {
                PagamentoId = pagamentoId,
                SessaoId = sessaoId,
                Valor = total,
                Status = 1
            };
        }


        public async Task SalvarPix(
     int pagamentoId,
     PixPagamentoDTO pix)
        {
            using var connection = _db.CreateConnection();

            await connection.ExecuteAsync(
                @"
                UPDATE Pagamento
                SET
                    Pag_TXID = @TXID,
                    Pag_QrCode = @QrCode,
                    Pag_CopiaECola = @CopiaECola,
                    Pag_MercadoPagoId = @MercadoPagoId
                WHERE Pag_Id = @PagamentoId
                ",
                new
                {
                    PagamentoId = pagamentoId,
                    TXID = pix.TXID,
                    QrCode = pix.QrCode,
                    CopiaECola = pix.CopiaECola,
                    MercadoPagoId = pix.MercadoPagoId
                });
        }

        public async Task<PagamentoDTO?> ObterPorMercadoPagoId(string mercadoPagoId)
        {
            using var connection = _db.CreateConnection();

            var sql = @"
                    SELECT
                        Pag_Id AS PagamentoId,
                        Pag_SessaoId AS SessaoId,
                        Pag_Valor AS Valor,
                        Pag_Status AS Status,
                        Pag_TXID AS TXID,
                        Pag_MercadoPagoId AS MercadoPagoId
                    FROM Pagamento
                    WHERE Pag_MercadoPagoId = @MercadoPagoId
                ";

            return await connection.QueryFirstOrDefaultAsync<PagamentoDTO>(
                sql,
            new
            {
                MercadoPagoId = mercadoPagoId
            });
        }
    }
}