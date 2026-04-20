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
            INSERT INTO usuarios (nome, email, senha_hash)
            VALUES (@Nome, @Email, @SenhaHash);
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
            UPDATE usuarios
            SET plano = @Plano,
                updated_at = NOW()
            WHERE email = @Email;
        ";

            await conn.ExecuteAsync(sql, new
            {
                Plano = plano,
                Email = email
            });
        }
    }
}
