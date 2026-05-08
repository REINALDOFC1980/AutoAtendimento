
using AutoAtedimento.API.DTO.AutoAtedimento.API.DTO;
using AutoAtedimento.API.Repositories;

namespace AutoAtedimento.API.Services
{
    public class PagamentoService
    {
        private readonly PagamentoRepository _repository;

        public PagamentoService(PagamentoRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagamentoDTO> GerarPix(int sessaoId)
        {
            return await _repository.GerarPix(sessaoId);
        }

        public async Task ConfirmarPagamento(int pagamentoId)
        {
            await _repository.ConfirmarPagamento(pagamentoId);
        }
    }
}