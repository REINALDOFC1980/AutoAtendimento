using AutoAtedimento.API.ENUM;

namespace AutoAtedimento.API.DTO
{
    public class PedidoDetalheDTO
    {
        public int PedidoId { get; set; }
        public int Mesa { get; set; }
        public DateTime DataHora { get; set; }

        public StatusPedido Status { get; set; }

        public List<PedidoItemDetalheDTO> Itens { get; set; } = new();
    }
}
