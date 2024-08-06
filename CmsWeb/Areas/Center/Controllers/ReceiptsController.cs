using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CmsDataAccess.DbModels;
using CmsDataAccess.Utils.FilesUtils;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CmsWeb.Areas.Center.Controllers
{
    [Area("CenterAdmin")]
    public class ReceiptsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ReceiptsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            return View("CenterAdmin/_Receipt", await _context.Receipt.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipt = await _context.Receipt
                .FirstOrDefaultAsync(m => m.Id == id);
            if (receipt == null)
            {
                return NotFound();
            }

            return View(receipt);
        }

        public IActionResult Create()
        {
            return View("CenterAdmin/ReceiptEditor");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Receipt receipt)
        {
            if (!ModelState.IsValid)
            {
                return View("CenterAdmin/ReceiptEditor", receipt);
            }

            string uniqueFilename = FileHandler.SaveUploadedFile(receipt.ReceiptPhoto);

            var newReceipt = new Receipt
            {
                Id = Guid.NewGuid(),
                CompanyName = receipt.CompanyName,
                ReceiptDate = receipt.ReceiptDate,
                TotalAmount = receipt.TotalAmount,
                TotalPrice = receipt.TotalPrice,
                ReceiptPhotoPath = FileHandler.getUploadfolder(),
                PhotoName = uniqueFilename
            };

            _context.Add(newReceipt);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DownloadPhoto(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound();
            }

            var filePath = Path.Combine(FileHandler.getUploadfolder(), fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var mimeType = "application/octet-stream"; // Default MIME type

            // You might want to determine the MIME type based on the file extension
            // and set it accordingly for better browser handling

            return File(fileBytes, mimeType, fileName);
        }

        [HttpGet]
        public async Task<IActionResult> GetReceipt(Guid id)
        {
            var receipt = await _context.Receipt.FindAsync(id);
            if (receipt == null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = receipt.Id,
                companyName = receipt.CompanyName,
                receiptDate = receipt.ReceiptDate.ToString("yyyy-MM-ddTHH:mm"),
                totalAmount = receipt.TotalAmount,
                totalPrice = receipt.TotalPrice,
                receiptPhoto = receipt.PhotoName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CompanyName,ReceiptDate,TotalAmount,TotalPrice")] Receipt receipt, IFormFile? ReceiptPhoto)
        {
            if (id != receipt.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingReceipt = await _context.Receipt.FindAsync(id);
                    if (existingReceipt == null)
                    {
                        return NotFound();
                    }

                    existingReceipt.CompanyName = receipt.CompanyName;
                    existingReceipt.ReceiptDate = receipt.ReceiptDate;
                    existingReceipt.TotalAmount = receipt.TotalAmount;
                    existingReceipt.TotalPrice = receipt.TotalPrice;

                    if (ReceiptPhoto != null && ReceiptPhoto.Length > 0)
                    {
                        existingReceipt.PhotoName = FileHandler.UpdateImageFile(existingReceipt.PhotoName, ReceiptPhoto);
                    }

                    _context.Update(existingReceipt);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceiptExists(receipt.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var receipt = await _context.Receipt.FindAsync(id);
            if (receipt != null)
            {
                FileHandler.DeleteImageFile(receipt.PhotoName);
                _context.Receipt.Remove(receipt);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReceiptExists(Guid id)
        {
            return _context.Receipt.Any(e => e.Id == id);
        }
    }
}