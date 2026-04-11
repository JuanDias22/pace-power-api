using Microsoft.AspNetCore.Mvc;
using PacePower.API.Application.Services;
namespace PacePower.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadsController : ControllerBase
    {
        private readonly LeadService _service;

        public LeadsController(LeadService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Lead lead)
        {
            var result = await _service.Create(lead);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var leads = await _service.GetAll();
            return Ok(leads);
        }
    }
}