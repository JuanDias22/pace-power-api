namespace PacePower.API.Controllers
{
    using Npgsql;
    using Dapper;

    public class UserRepository
    {
        private readonly IConfiguration _config;

        public UserRepository(IConfiguration config)
        {
            _config = config;
        }

        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        public async Task CreateAsync(string nome, string email, string senhaHash)
        {
            using var conn = GetConnection();

            var sql = @"
        INSERT INTO ""Users"" (""Nome"", ""Email"", ""SenhaHash"", ""CreatedAt"", ""UpdatedAt"")
        VALUES (@Nome, @Email, @SenhaHash, NOW(), NOW());
    ";

            await conn.ExecuteAsync(sql, new
            {
                Nome = nome,
                Email = email,
                SenhaHash = senhaHash
            });
        }

        public async Task AtualizarPlanosAsync(string email, string plano)
        {
            using var conn = GetConnection();

            var sql = @"
        UPDATE ""Users""
        SET ""Plano"" = @Plano,
            ""UpdatedAt"" = NOW()
        WHERE ""Email"" = @Email;
    ";

            await conn.ExecuteAsync(sql, new
            {
                Plano = plano,
                Email = email
            });
        
        }
    }
}
