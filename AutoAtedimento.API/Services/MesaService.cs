using AutoAtedimento.API.DTO;
using AutoAtedimento.API.Models;
using AutoAtedimento.API.Repositories;

namespace AutoAtedimento.API.Services
{
    public class MesaService
    {
        private readonly MesaRepository _repository;

        public MesaService(MesaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MesaDTO>> Listar()
        {
            return await _repository.Listar();
        }

        public async Task<int> Criar(MesaModel model)
        {
            return await _repository.Criar(model);
        }

        public async Task Inativar(int id)
        {
            await _repository.Inativar(id);
        }
    }
}