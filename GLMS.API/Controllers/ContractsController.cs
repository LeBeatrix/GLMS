using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GLMS.API.Data;
using GLMS.API.Models;
using Microsoft.AspNetCore.Hosting;

namespace GLMS.API.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ContractsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Contracts.Include(c => c.Client);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientId,StartDate,EndDate,Status,ServiceLevel")] Contract contract, IFormFile? agreementFile)
        {
            // PDF Validation
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
            // Date Validation
            if (contract.EndDate <= contract.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after the start date.");
            }

            if (ModelState.IsValid)
            {
                //Save PDF
                if (agreementFile != null)
                {
                    string uploadsFolder = Path.Combine(
                        _environment.WebRootPath,
                        "uploads",
                        "contracts"
                    );

                    string uniqueFileName =
                        Guid.NewGuid().ToString() + ".pdf";

                    string filePath =
                        Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await agreementFile.CopyToAsync(fileStream);
                    }

                    contract.AgreementFilePath =
                "/uploads/contracts/" + uniqueFileName;
                }

                _context.Add(contract);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);

            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,StartDate,EndDate,Status,ServiceLevel,AgreementFilePath")] Contract contract,
        IFormFile? agreementFile)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            // PDF Validation
            if (string.IsNullOrEmpty(contract.AgreementFilePath) &&
                (agreementFile == null || agreementFile.Length == 0))
            {
                ModelState.AddModelError("", "A signed agreement PDF must be uploaded.");
            }
            else if (agreementFile != null)
            {
                var extension = Path.GetExtension(agreementFile.FileName).ToLower();

                if (extension != ".pdf")
                {
                    ModelState.AddModelError("", "Only PDF files are allowed.");
                }
            }

            // Date Validation
            if (contract.EndDate <= contract.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after the start date.");
            }
            if (ModelState.IsValid)
            { 
                try
                { 
                // Upload new file
                if (agreementFile != null)
                {
                    string uploadsFolder = Path.Combine(
                        _environment.WebRootPath,
                        "uploads",
                        "contracts"
                    );

                        string uniqueFileName =
                        Guid.NewGuid().ToString() + ".pdf";

                        string filePath =
                            Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await agreementFile.CopyToAsync(fileStream);
                        }

                        contract.AgreementFilePath =
                    "/uploads/contracts/" + uniqueFileName;
                    }

                    _context.Update(contract);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);

            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}
