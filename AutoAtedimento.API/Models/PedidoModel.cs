namespace AutoAtedimento.API.Models
{
    public class PedidoModel
    {
        //public int Ped_MesaId { get; set; }
        public int Ped_SessaoId { get; set; }
        public string? Ped_Observacao { get; set; }
        public List<PedidoItemModel> Itens { get; set; } = new();
    }
}
