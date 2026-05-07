using AutoAtedimento.API.ENUM;

namespace AutoAtedimento.API.DTO
{
    public class PedidoCozinhaDTO
    {
        public int PedidoId { get; set; }

        public int Mesa { get; set; }

        public DateTime DataHora { get; set; }

        public int StatusId { get; set; }

        public string? Status { get; set; }

        public List<PedidoItemDTO> Itens { get; set; } = new();
    }
}
