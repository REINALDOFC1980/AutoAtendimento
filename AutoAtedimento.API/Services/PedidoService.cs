using AutoAtedimento.API.DTO;
using AutoAtedimento.API.DTO.AutoAtedimento.API.DTO;
using AutoAtedimento.API.ENUM;
using AutoAtedimento.API.Models;
using AutoAtedimento.API.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace AutoAtedimento.API.Services
{
    public class PedidoService
    {
        private readonly PedidoRepository _repository;
        private readonly IHubContext<PedidoHub> _hub;

        public PedidoService(PedidoRepository repository, IHubContext<PedidoHub> hub)
        {
            _repository = repository;
            _hub = hub;
        }

        public async Task<int> CriarPedido(PedidoModel pedido)
        {
            var pedidoId = await _repository.CriarPedido(pedido);

            var pedidoCompleto = await _repository.ObterPedidoPorId(pedidoId);

            await _hub.Clients.All.SendAsync("NovoPedido", pedidoCompleto);

            return pedidoId;
        }

        public async Task<IEnumerable<PedidoCozinhaDTO>> ListarPedidosCozinha()
        {
            return await _repository.ListarPedidosCozinha();
        }

        public async Task AtualizarStatus(int pedidoId, StatusPedido status)
        {
            await _repository.AtualizarStatus(pedidoId, status);

            var pedidoAtualizado = await _repository.ObterPedidoPorId(pedidoId);

            await _hub.Clients.All.SendAsync("PedidoAtualizado", pedidoAtualizado);
        }

        public async Task<PedidoDetalheDTO> ObterPedidoPorId(int pedidoId)
        {
            return await _repository.ObterPedidoPorId(pedidoId);
        }

        public async Task<IEnumerable<PedidoDetalheDTO>> ObterPedidosPorMesa(int mesaId)
        {
            return await _repository.ObterPedidosPorMesa(mesaId);
        }

        public async Task FinalizarPedido(int pedidoId)
        {
            await _repository.FinalizarPedido(pedidoId);

            var pedido = await _repository.ObterPedidoPorId(pedidoId);

            await _hub.Clients.Group("cozinha")
                .SendAsync("PedidoFinalizado", pedido);
        }

        public async Task CancelarPedido(int pedidoId)
        {
            await _repository.CancelarPedido(pedidoId);

            await _hub.Clients.Group("cozinha")
                .SendAsync("PedidoCancelado", new { pedidoId });
        }


        public async Task<PedidoTotalDTO> ObterTotalPedido(int pedidoId)
        {
            return await _repository.ObterTotalPedido(pedidoId);
        }
    }


}
