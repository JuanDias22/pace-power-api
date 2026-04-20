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
    public async Task<IActionResult> CriarPagamento([FromBody] PaymentsRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var accessToken = _config["MercadoPago:AccessToken"];

        if (string.IsNullOrEmpty(accessToken))
            return StatusCode(500, new { erro = "Access Token não configurado" });

        var plano = request.Plano?.Trim().ToLower();

        var valor = plano switch
        {
            "basico" => 79,
            "intermediario" => 119,
            "premium" => 149,
            _ => 0
        };

        if (valor == 0)
        {
            return BadRequest(new { erro = "Plano inválido" });
        }

        var body = new
        {
            items = new[]
            {
                new
                {
                    title = $"Plano {plano}",
                    quantity = 1,
                    currency_id = "BRL",
                    unit_price = valor
                }
            },
            payer = new
            {
                email = request.Email
            },
            notification_url = _config["MercadoPago:WebhookUrl"]
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
            return StatusCode((int)response.StatusCode, new
            {
                erro = "Erro ao criar pagamento",
                detalhe = responseContent
            });
        }

        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);

        var preferenceId = result.GetProperty("id").GetString();
        var initPoint = result.GetProperty("init_point").GetString();

        await _paymentRepository.CreateAsync(
            request.Email,
            plano,
            valor,
            "pendente",
            preferenceId
        );

        return Ok(new
        {
            url = initPoint
        });
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
        {
        string body;

        using (var reader = new StreamReader(Request.Body))
            body = await reader.ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(body))
            return BadRequest(new { erro = "Body vazio" });

        JsonElement json;

        try
        {
            json = JsonSerializer.Deserialize<JsonElement>(body);
    }
        catch (JsonException ex)
            {
            _logger.LogWarning(ex, "JSON inválido recebido no webhook");
            return BadRequest(new { erro = "JSON inválido" });
        }

        if (!json.TryGetProperty("data", out var data) ||
            !data.TryGetProperty("id", out var idProp))
        {
            return BadRequest(new { erro = "Payload inválido" });
        }

        var idPagamento = idProp.GetString();

        if (string.IsNullOrEmpty(idPagamento))
            return BadRequest(new { erro = "ID do pagamento inválido" });

        try
        {
            var accessToken = _config["MercadoPago:AccessToken"];

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogError("AccessToken não configurado");
                return StatusCode(500, new { erro = "Configuração inválida" });
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            var response = await _httpClient.GetAsync(
                $"https://api.mercadopago.com/v1/payments/{idPagamento}"
            );

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Erro ao consultar pagamento no Mercado Pago. Status: {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode);
        }

            var content = await response.Content.ReadAsStringAsync();
            var pagamento = JsonSerializer.Deserialize<JsonElement>(content);

            var status = pagamento.GetProperty("status").GetString();
            var email = pagamento.GetProperty("payer").GetProperty("email").GetString();

            var registro = await _paymentRepository.ObterIdMercadoPagoAsync(idPagamento);

            if (registro == null)
            {
                _logger.LogWarning("Pagamento não encontrado no banco. Id: {Id}", idPagamento);
                return NotFound();
    }
            var plano = registro.Plano;
}

public class PaymentRequest
{
    public string Plano { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
}