using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PacePower.API.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly PaymentsRepository _paymentRepository;
    private readonly UserRepository _userRepository;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(HttpClient httpClient, IConfiguration config, PaymentsRepository paymentRepository, UserRepository userRepository, ILogger<PaymentsController> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _logger = logger;
    }


    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
    {
        var accessToken = _config["MercadoPago:AccessToken"];

        Console.WriteLine($"Plano recebido: {request.Plano}");

        var plano = request.Plano?.Replace("á", "a");

        var valor = plano switch
        {
            "Basico" => 79,
            "Intermediario" => 119,
            "Premium" => 149,
            _ => 0
        };

        if (valor == 0)
        {
            return BadRequest(new { erro = "Plano inválido" });
        }

        var body = new
        {
            transaction_amount = valor,
            description = plano,
            payment_method_id = "pix",
            payer = new
            {
                email = request.Email
            }
        };

        var json = JsonSerializer.Serialize(body);

        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        var response = await _httpClient.PostAsync(
            "https://api.mercadopago.com/checkout/preferences",
            httpContent
        );

        var responseContent = await response.Content.ReadAsStringAsync();

        Console.WriteLine("Retorno Mercado Pago:");
        Console.WriteLine(responseContent);

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest(new
            {
                erro = "Erro ao criar pagamento",
                detalhe = responseContent
            });
        }

        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

        if (!result.TryGetProperty("init_point", out var initPoint))
        {
            return BadRequest(new
            {
                erro = "Resposta inválida do Mercado Pago",
                detalhe = responseContent
            });
        }

        var url = initPoint.GetString();

        return Ok(new { url });
    }
}

public class PaymentRequest
{
    public string Plano { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
}