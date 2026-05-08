namespace AutoAtedimento.API.Models
{
    public class MesaModel
    {
        public int Mes_Id { get; set; }

        public int Mes_Numero { get; set; }

        public string? Mes_Descricao { get; set; }

        public bool Mes_Ativa { get; set; } = true;
    }
}