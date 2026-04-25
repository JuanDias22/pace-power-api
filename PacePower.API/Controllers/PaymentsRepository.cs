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
            INSERT INTO ""Payments""
            (""Email"", ""Plano"", ""Valor"", ""Status"", ""MercadoPagoId"", ""CreatedAt"", ""UpdatedAt"")
            VALUES (@Email, @Plano, @Valor, @Status, @MercadoPagoId, NOW(), NOW())
            RETURNING ""Id"";
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
            UPDATE ""Payments""
            SET ""Status"" = @Status,
                ""UpdatedAt"" = NOW()
            WHERE ""MercadoPagoId"" = @MercadoPagoId;
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
            SELECT ""Email"", ""Plano"", ""Valor"", ""Status"", ""MercadoPagoId""
            FROM ""Payments""
            WHERE ""MercadoPagoId"" = @Id
            LIMIT 1;
        ";

        return await conn.QueryFirstOrDefaultAsync(sql, new
        {
            Id = id
        });
    }
}