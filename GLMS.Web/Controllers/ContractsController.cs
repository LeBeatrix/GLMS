using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GLMS.Web.Models;
using GLMS.Web.ApiServices;

namespace GLMS.Web.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ContractApiService _contractApiService;
        private readonly ClientApiService _clientApiService;

        public ContractsController(
            ContractApiService contractApiService,
            ClientApiService clientApiService)
        {
            _contractApiService = contractApiService;
            _clientApiService = clientApiService;
        }

        public async Task<IActionResult> Index()
        {
            var contracts = await _contractApiService.GetContractsAsync();
            return View(contracts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _contractApiService.GetContractByIdAsync(id.Value);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        public async Task<IActionResult> Create()
        {
            var clients = await _clientApiService.GetClientsAsync();
            ViewData["ClientId"] = new SelectList(clients, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,ClientId,StartDate,EndDate,Status,ServiceLevel")]
            Contract contract,
            IFormFile? agreementFile)
        {
            if (agreementFile == null || agreementFile.Length == 0)
            {
                ModelState.AddModelError("", "A signed agreement PDF must be uploaded.");
            }
            else
            {
                var extension = Path.GetExtension(agreementFile.FileName).ToLower();

                if (extension != ".pdf")
                {
                    ModelState.AddModelError("", "Only PDF files are allowed.");
                }
            }

            if (contract.EndDate <= contract.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after the start date.");
            }

            if (ModelState.IsValid)
            {
                var success = await _contractApiService.CreateContractAsync(contract, agreementFile!);

                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed to create contract through the API.");
            }

            var clients = await _clientApiService.GetClientsAsync();
            ViewData["ClientId"] = new SelectList(clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _contractApiService.GetContractByIdAsync(id.Value);

            if (contract == null)
            {
                return NotFound();
            }

            var clients = await _clientApiService.GetClientsAsync();
            ViewData["ClientId"] = new SelectList(clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,ClientId,StartDate,EndDate,Status,ServiceLevel,AgreementFilePath")]
            Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (contract.EndDate <= contract.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after the start date.");
            }

            if (ModelState.IsValid)
            {
                var success = await _contractApiService.UpdateContractStatusAsync(
                    contract.Id,
                    contract.Status
                );

                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed to update contract status through the API.");
            }

            var clients = await _clientApiService.GetClientsAsync();
            ViewData["ClientId"] = new SelectList(clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _contractApiService.GetContractByIdAsync(id.Value);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _contractApiService.DeleteContractAsync(id);

            if (!success)
            {
                ModelState.AddModelError("", "Failed to delete contract through the API.");

                var contract = await _contractApiService.GetContractByIdAsync(id);

                if (contract == null)
                {
                    return NotFound();
                }

                return View("Delete", contract);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}