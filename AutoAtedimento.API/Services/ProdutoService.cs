using AutoAtedimento.API.DTO;
using AutoAtedimento.API.Models;
using AutoAtedimento.API.Repositories;

namespace AutoAtedimento.API.Services
{
    public class ProdutoService
    {
        private readonly ProdutoRepository _repository;

        public ProdutoService(ProdutoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProdutoDTO>> Listar()
        {
            return await _repository.Listar();
        }

        public async Task<ProdutoDTO> ObterPorId(int id)
        {
            return await _repository.ObterPorId(id);
        }

        public async Task<int> Criar(ProdutoModel model)
        {
            return await _repository.Criar(model);
        }

        public async Task Atualizar(int id, ProdutoModel model)
        {
            await _repository.Atualizar(id, model);
        }

        public async Task Inativar(int id)
        {
            await _repository.Inativar(id);
        }

        public async Task AtualizarImagem(int id, string nomeArquivo)
        {
            await _repository.AtualizarImagem(id, nomeArquivo);
        }
    }
}