using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GLMS.Web.Data;
using GLMS.Web.Models;
using GLMS.Web.Services;

namespace GLMS.Web.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ServiceRequestValidator _validator;
        private readonly CurrencyService _currencyService;

        public ServiceRequestsController(
            ApplicationDbContext context,
            ServiceRequestValidator validator,
            CurrencyService currencyService)
        {
            _context = context;
            _validator = validator;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var serviceRequests = _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client);

            return View(await serviceRequests.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateContractsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContractId,Description,CostUSD,CostZAR,Status")] ServiceRequest serviceRequest)
        {
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == serviceRequest.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError("", "Selected contract could not be found.");
            }
            else if (!_validator.CanCreateServiceRequest(contract))
            {
                ModelState.AddModelError("", _validator.GetValidationMessage(contract));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    decimal exchangeRate = await _currencyService.GetUsdToZarRateAsync();
                    serviceRequest.CostZAR = Math.Round(serviceRequest.CostUSD * exchangeRate, 2);

                    _context.Add(serviceRequest);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "Currency API is currently unavailable. Please try again later.");
                }
            }

            await PopulateContractsDropDownList(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null) return NotFound();

            await PopulateContractsDropDownList(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ContractId,Description,CostUSD,CostZAR,Status")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id) return NotFound();

            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == serviceRequest.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError("", "Selected contract could not be found.");
            }
            else if (!_validator.CanCreateServiceRequest(contract))
            {
                ModelState.AddModelError("", _validator.GetValidationMessage(contract));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    decimal exchangeRate = await _currencyService.GetUsdToZarRateAsync();
                    serviceRequest.CostZAR = Math.Round(serviceRequest.CostUSD * exchangeRate, 2);

                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.Id)) return NotFound();
                    throw;
                }
                catch
                {
                    ModelState.AddModelError("", "Currency API is currently unavailable. Please try again later.");
                }
            }

            await PopulateContractsDropDownList(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateContractsDropDownList(object? selectedContract = null)
        {
            var contracts = await _context.Contracts
                .Include(c => c.Client)
                .ToListAsync();

            ViewData["ContractId"] = new SelectList(
                contracts,
                "Id",
                "DisplayName",
                selectedContract
            );
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.Id == id);
        }
    }
}