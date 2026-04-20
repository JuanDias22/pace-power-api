using Npgsql;
using Dapper;

public class PaymentsRepository
{
    private readonly IConfiguration _config;

    public PaymentsRepository(IConfiguration config)
    {
        _config = config;
    }

    private NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
    }
    public async Task<int> CreateAsync(
        string email,
        string plano,
        decimal valor,
        string status,
        string mercadoPagoId)
    {
        using var conn = GetConnection();

        var sql = @"
            INSERT INTO pagamentos (email, plano, valor, status, mercado_pago_id)
            VALUES (@Email, @Plano, @Valor, @Status, @MercadoPagoId)
            RETURNING id;
        ";

        return await conn.ExecuteScalarAsync<int>(sql, new
        {
            Email = email,
            Plano = plano,
            Valor = valor,
            Status = status,
            MercadoPagoId = mercadoPagoId
        });
    }

    public async Task AtualizarStatusAsync(string mercadoPagoId, string status)
    {
        using var conn = GetConnection();

        var sql = @"
            UPDATE pagamentos
            SET status = @Status,
                updated_at = NOW()
            WHERE mercado_pago_id = @MercadoPagoId;
        ";

        await conn.ExecuteAsync(sql, new
        {
            Status = status,
            MercadoPagoId = mercadoPagoId
        });
    }

    public async Task<dynamic?> ObterIdMercadoPagoAsync(string id)
    {
        using var conn = GetConnection();

        var sql = @"
            SELECT email, plano, valor, status, mercado_pago_id
            FROM pagamentos
            WHERE mercado_pago_id = @Id
            LIMIT 1;
        ";

        return await conn.QueryFirstOrDefaultAsync(sql, new
        {
            Id = id
        });
    }
}