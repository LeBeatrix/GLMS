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
        private readonly IWebHostEnvironment _environment;

        public ContractsController(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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

            var contracts = await query
                .Select(c => new
                {
                    c.Id,
                    c.ClientId,
                    ClientName = c.Client != null ? c.Client.Name : "No Client",
                    c.StartDate,
                    c.EndDate,
                    Status = c.Status,
                    c.ServiceLevel,
                    c.AgreementFilePath
                })
                .ToListAsync();

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

            return Ok(new
            {
                contract.Id,
                contract.ClientId,
                ClientName = contract.Client != null ? contract.Client.Name : "No Client",
                contract.StartDate,
                contract.EndDate,
                Status = contract.Status,
                contract.ServiceLevel,
                contract.AgreementFilePath
            });
        }

        // POST: /api/contracts
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateContract([FromForm] CreateContractDto dto)
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

            if (dto.AgreementFile == null || dto.AgreementFile.Length == 0)
            {
                return BadRequest(new { message = "A signed agreement PDF must be uploaded." });
            }

            var extension = Path.GetExtension(dto.AgreementFile.FileName).ToLower();

            if (extension != ".pdf")
            {
                return BadRequest(new { message = "Only PDF files are allowed." });
            }

            string uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "uploads",
                "contracts"
            );

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + ".pdf";

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await dto.AgreementFile.CopyToAsync(fileStream);
            }

            var contract = new Contract
            {
                ClientId = dto.ClientId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                ServiceLevel = dto.ServiceLevel,
                AgreementFilePath = "/uploads/contracts/" + uniqueFileName
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetContractById),
                new { id = contract.Id },
                new
                {
                    contract.Id,
                    contract.ClientId,
                    contract.StartDate,
                    contract.EndDate,
                    Status = contract.Status,
                    contract.ServiceLevel,
                    contract.AgreementFilePath
                }
            );
        }

        // PUT: /api/contracts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContract(int id, UpdateContractDto dto)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound(new { message = "Contract not found." });
            }

            if (dto.EndDate <= dto.StartDate)
            {
                return BadRequest(new { message = "End date must be after the start date." });
            }

            var clientExists = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId);

            if (!clientExists)
            {
                return BadRequest(new { message = "Selected client does not exist." });
            }

            contract.ClientId = dto.ClientId;
            contract.StartDate = dto.StartDate;
            contract.EndDate = dto.EndDate;
            contract.Status = dto.Status;
            contract.ServiceLevel = dto.ServiceLevel;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                contract.Id,
                contract.ClientId,
                contract.StartDate,
                contract.EndDate,
                Status = contract.Status,
                contract.ServiceLevel,
                contract.AgreementFilePath
            });
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

        // DELETE: /api/contracts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return NotFound(new
                {
                    message = "Contract not found."
                });
            }

            _context.Contracts.Remove(contract);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Contract deleted successfully."
            });
        }
    }
}