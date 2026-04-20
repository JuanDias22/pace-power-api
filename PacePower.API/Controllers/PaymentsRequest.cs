using System.ComponentModel.DataAnnotations;

public class PaymentsRequest
{
    [Required]
    [RegularExpression("Basico|Intermediario|Premium")]
    public string Plano { get; set; }

    [Required]
    [MinLength(3)]
    public string Nome { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}

public class MercadoPagoWebhook
{
    public string Type { get; set; }
    public WebhookData Data { get; set; }
}

public class WebhookData
{
    public string Id { get; set; }
}