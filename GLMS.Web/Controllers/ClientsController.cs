using GLMS.Web.ApiServices;
using GLMS.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Web.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ClientApiService _clientApiService;

        public ClientsController(ClientApiService clientApiService)
        {
            _clientApiService = clientApiService;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _clientApiService.GetClientsAsync();
            return View(clients);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientApiService.GetClientByIdAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ContactDetails,Region")] Client client)
        {
            if (ModelState.IsValid)
            {
                var success = await _clientApiService.CreateClientAsync(client);

                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed to create client through the API.");
            }

            return View(client);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientApiService.GetClientByIdAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ContactDetails,Region")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var success = await _clientApiService.UpdateClientAsync(client);

                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", "Failed to update client through the API.");
            }

            return View(client);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientApiService.GetClientByIdAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _clientApiService.DeleteClientAsync(id);

            if (!success)
            {
                ModelState.AddModelError("", "Failed to delete client through the API.");

                var client = await _clientApiService.GetClientByIdAsync(id);

                if (client == null)
                {
                    return NotFound();
                }

                return View("Delete", client);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}