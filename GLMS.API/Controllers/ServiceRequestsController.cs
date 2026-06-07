using GLMS.API.Data;
using GLMS.API.Models;
using GLMS.API.Services;
using GLMS.API.Interfaces;
using GLMS.API.Observers;
using GLMS.API.Factories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GLMS.API.DTOs;

namespace GLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ServiceRequestValidator _validator;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IServiceRequestObserver _observer;
        private readonly IServiceRequestFactory _factory;

        public ServiceRequestsController(
            ApplicationDbContext context,
            ServiceRequestValidator validator,
            ICurrencyConverter currencyConverter,
            IServiceRequestObserver observer,
            IServiceRequestFactory factory)
        {
            _context = context;
            _validator = validator;
            _currencyConverter = currencyConverter;
            _observer = observer;
            _factory = factory;
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceRequests()
        {
            var requests = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .Select(s => new
                {
                    s.Id,
                    s.ContractId,
                    ContractName = s.Contract != null ? s.Contract.DisplayName : "No Contract",
                    s.Description,
                    s.CostUSD,
                    s.CostZAR,
                    s.Status
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceRequestById(int id)
        {
            var request = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (request == null)
            {
                return NotFound(new { message = "Service request not found." });
            }

            return Ok(new
            {
                request.Id,
                request.ContractId,
                ContractName = request.Contract != null ? request.Contract.DisplayName : "No Contract",
                request.Description,
                request.CostUSD,
                request.CostZAR,
                request.Status
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceRequest(CreateServiceRequestDto serviceRequest)
        {
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == serviceRequest.ContractId);

            if (contract == null)
            {
                return BadRequest(new { message = "Selected contract could not be found." });
            }

            if (!_validator.CanCreateServiceRequest(contract))
            {
                return BadRequest(new { message = _validator.GetValidationMessage(contract) });
            }

            var validStatuses = new[] { "Pending", "Approved", "Rejected", "Completed" };

            if (!validStatuses.Contains(serviceRequest.Status))
            {
                return BadRequest(new { message = "Invalid service request status." });
            }

            var newRequest = _factory.Create(
                serviceRequest.ContractId,
                serviceRequest.Description,
                serviceRequest.CostUSD,
                serviceRequest.Status
            );

            newRequest.CostZAR =
                await _currencyConverter.ConvertAsync(newRequest.CostUSD);

            _context.ServiceRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetServiceRequestById),
                new { id = newRequest.Id },
                newRequest
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRequest(
            int id,
            UpdateServiceRequestDto dto)
        {
            var existingRequest = await _context.ServiceRequests.FindAsync(id);

            if (existingRequest == null)
            {
                return NotFound(new { message = "Service request not found." });
            }

            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == dto.ContractId);

            if (contract == null)
            {
                return BadRequest(new { message = "Selected contract could not be found." });
            }

            if (!_validator.CanCreateServiceRequest(contract))
            {
                return BadRequest(new { message = _validator.GetValidationMessage(contract) });
            }

            var validStatuses = new[] { "Pending", "Approved", "Rejected", "Completed" };

            if (!validStatuses.Contains(dto.Status))
            {
                return BadRequest(new { message = "Invalid service request status." });
            }

            existingRequest.ContractId = dto.ContractId;
            existingRequest.Description = dto.Description;
            existingRequest.CostUSD = dto.CostUSD;
            existingRequest.Status = dto.Status;
            existingRequest.CostZAR = await _currencyConverter.ConvertAsync(dto.CostUSD);

            await _context.SaveChangesAsync();

            _observer.Update(existingRequest);

            return Ok(new
            {
                existingRequest.Id,
                existingRequest.ContractId,
                existingRequest.Description,
                existingRequest.CostUSD,
                existingRequest.CostZAR,
                existingRequest.Status
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);

            if (request == null)
            {
                return NotFound(new { message = "Service request not found." });
            }

            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Service request deleted successfully." });
        }
    }
}