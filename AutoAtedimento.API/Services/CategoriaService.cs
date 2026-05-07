using AutoAtedimento.API.DTO;
using AutoAtedimento.API.DTO.AutoAtedimento.API.DTO;
using AutoAtedimento.API.Models;
using AutoAtedimento.API.Repositories;

namespace AutoAtedimento.API.Services
{
    public class CategoriaService
    {
        private readonly CategoriaRepository _repository;

        public CategoriaService(CategoriaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CategoriaDTO>> Listar()
        {
            return await _repository.Listar();
        }

        public async Task<CategoriaDTO> ObterPorId(int id)
        {
            return await _repository.ObterPorId(id);
        }

        public async Task<int> Criar(CategoriaModel model)
        {
            return await _repository.Criar(model);
        }

        public async Task Atualizar(int id, CategoriaModel model)
        {
            await _repository.Atualizar(id, model);
        }

        public async Task Inativar(int id)
        {
            await _repository.Inativar(id);
        }
    }
}