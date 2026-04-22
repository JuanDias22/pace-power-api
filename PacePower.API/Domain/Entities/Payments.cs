namespace PacePower.API.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Plano { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
        public string MercadoPagoId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int? UserId { get; set; } 
    }
}
