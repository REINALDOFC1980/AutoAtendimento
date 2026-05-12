namespace AutoAtedimento.API.DTO
{
    public class PagamentoWebhookDTO
    {
        public string? Action { get; set; }

        public WebhookDataDTO? Data { get; set; }
    }

    public class WebhookDataDTO
    {
        public string? Id { get; set; }
    }
}