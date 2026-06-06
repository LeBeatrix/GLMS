using GLMS.API.Data;
using GLMS.API.DTOs;
using GLMS.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/contracts
        [HttpGet]
        public async Task<IActionResult> GetContracts(
            string? status,
            DateTime? startDate,
            DateTime? endDate)
        {
            var query = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.Status.ToString() == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.EndDate <= endDate.Value);
            }

            var contracts = await query.ToListAsync();

            return Ok(contracts);
        }

        // GET: /api/contracts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContractById(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null)
            {
                return NotFound(new { message = "Contract not found." });
            }

            return Ok(contract);
        }

        // POST: /api/contracts
        [HttpPost]
        public async Task<IActionResult> CreateContract(CreateContractDto dto)
        {
            if (dto.EndDate <= dto.StartDate)
            {
                return BadRequest(new { message = "End date must be after the start date." });
            }

            var clientExists = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId);

            if (!clientExists)
            {
                return BadRequest(new { message = "Selected client does not exist." });
            }

            var contract = new Contract
            {
                ClientId = dto.ClientId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                ServiceLevel = dto.ServiceLevel,
                AgreementFilePath = dto.AgreementFilePath
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetContractById),
                new { id = contract.Id },
                contract
            );
        }

        // PATCH: /api/contracts/5/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateContractStatus(
            int id,
            UpdateContractStatusDto dto)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound(new { message = "Contract not found." });
            }

            contract.Status = dto.Status;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Contract status updated successfully.",
                contractId = contract.Id,
                newStatus = contract.Status.ToString()
            });
        }
    }
}