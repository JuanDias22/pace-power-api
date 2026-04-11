using Microsoft.EntityFrameworkCore;
using PacePower.API.Infrastructure.Data;

namespace PacePower.API.Application.Services
{
    public class LeadService
    {
        private readonly AppDbContext _context;

        public LeadService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Lead> Create(Lead lead)
        {
            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();
            return lead;
        }

        public async Task<List<Lead>> GetAll()
        {
            return await _context.Leads.ToListAsync();
        }
    }
}