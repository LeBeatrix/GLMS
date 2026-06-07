using GLMS.Web.ApiServices;
using GLMS.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS.Web.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ServiceRequestApiService _serviceRequestApiService;
        private readonly ContractApiService _contractApiService;

        public ServiceRequestsController(
            ServiceRequestApiService serviceRequestApiService,
            ContractApiService contractApiService)
        {
            _serviceRequestApiService = serviceRequestApiService;
            _contractApiService = contractApiService;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _serviceRequestApiService.GetServiceRequestsAsync();
            return View(requests);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _serviceRequestApiService.GetServiceRequestByIdAsync(id.Value);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateContractsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,ContractId,Description,CostUSD,CostZAR,Status")]
            ServiceRequest serviceRequest)
        {
            var validStatuses = new[] { "Pending", "Approved", "Rejected", "Completed" };

            if (!validStatuses.Contains(serviceRequest.Status))
            {
                ModelState.AddModelError("Status", "Invalid service request status.");
            }

            if (ModelState.IsValid)
            {
                var success = await _serviceRequestApiService.CreateServiceRequestAsync(serviceRequest);

                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed to create service request through the API. Check contract status and API availability.");
            }

            await PopulateContractsDropDownList(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _serviceRequestApiService.GetServiceRequestByIdAsync(id.Value);

            if (request == null)
            {
                return NotFound();
            }

            await PopulateContractsDropDownList(request.ContractId);
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,ContractId,Description,CostUSD,CostZAR,Status")]
            ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
            {
                return NotFound();
            }

            var validStatuses = new[] { "Pending", "Approved", "Rejected", "Completed" };

            if (!validStatuses.Contains(serviceRequest.Status))
            {
                ModelState.AddModelError("Status", "Invalid service request status.");
            }

            if (ModelState.IsValid)
            {
                var success = await _serviceRequestApiService.UpdateServiceRequestAsync(serviceRequest);

                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed to update service request through the API.");
            }

            await PopulateContractsDropDownList(serviceRequest.ContractId);
            return View(serviceRequest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _serviceRequestApiService.GetServiceRequestByIdAsync(id.Value);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _serviceRequestApiService.DeleteServiceRequestAsync(id);

            if (!success)
            {
                ModelState.AddModelError("", "Failed to delete service request through the API.");

                var request = await _serviceRequestApiService.GetServiceRequestByIdAsync(id);

                if (request == null)
                {
                    return NotFound();
                }

                return View("Delete", request);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateContractsDropDownList(object? selectedContract = null)
        {
            var contracts = await _contractApiService.GetContractsAsync();

            ViewData["ContractId"] = new SelectList(
                contracts,
                "Id",
                "DisplayName",
                selectedContract
            );
        }
    }
}