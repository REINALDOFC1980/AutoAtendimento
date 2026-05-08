using AutoAtedimento.API.DTO;
using AutoAtedimento.API.Repositories;

namespace AutoAtedimento.API.Services
{
    public class MesaSessaoService
    {
        private readonly MesaSessaoRepository _repository;

        public MesaSessaoService(MesaSessaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> AbrirSessao(int mesaId)
        {
            return await _repository.AbrirSessao(mesaId);
        }

        public async Task<FecharSessaoDTO> FecharSessao(int sessaoId)
        {
            return await _repository.FecharSessao(sessaoId);
        }
    }
}