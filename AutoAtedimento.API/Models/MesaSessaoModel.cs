namespace AutoAtedimento.API.Models
{
    namespace AutoAtedimento.API.Models
    {
        public class MesaSessaoModel
        {
            public int Ses_Id { get; set; }

            public int Ses_MesaId { get; set; }

            public DateTime Ses_DataAbertura { get; set; }

            public DateTime? Ses_DataFechamento { get; set; }

            public int Ses_Status { get; set; }
        }
    }
}
