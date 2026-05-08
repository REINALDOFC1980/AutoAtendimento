namespace AutoAtedimento.API.DTO
{
    public class MesaDTO
    {
        public int Id { get; set; }

        public int Numero { get; set; }

        public string? Descricao { get; set; }

        public bool Ativa { get; set; }

        public string? QRCodeUrl { get; set; }
    }
}