    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using CmsDataAccess.DbModels;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using CmsWeb.Areas.CenterEmployeeReception.Models;
    using CmsDataAccess.MobileViewModels.AuthModels;
    using PdfSharpCore.Drawing;
    using PdfSharpCore.Pdf;
    using System.IO;

namespace YourAppName.Controllers
    {
        [Area("CenterEmployeeReception")]
        public class StoreBillController : Controller
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<StoreBillController> _logger;

            public StoreBillController(ApplicationDbContext context, ILogger<StoreBillController> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
            {
                var bills = await _context.Bill
                            .OrderByDescending(b => b.BillDate)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
                return View("CenterEmployeeReception/_StoreBills", bills);
            }



            [HttpGet]
            public async Task<IActionResult> GetBillProducts(Guid billId)
            {
                try
                {
                    var billProducts = await _context.BillProduct
                        .Where(bp => bp.BillID == billId)
                        .Include(bp => bp.Product)
                        .ThenInclude(p => p.ProductTranslation)
                        .ToListAsync();

                    Console.WriteLine($"Retrieved {billProducts.Count} products for bill {billId}");

                    if (billProducts == null || !billProducts.Any())
                    {
                        Console.WriteLine($"No products found for bill {billId}");
                        return Json(new { success = false, message = "No products found for this bill." });
                    }

                    var productData = billProducts.Select(bp => new
                    {
                        Name = bp.Product?.ProductTranslation?.FirstOrDefault(pt => pt.LangCode == "en-US")?.Name ?? "Unknown",
                        Quantity = bp.Quantity,
                        Price = bp.Price,
                        Discount = bp.Discount,
                        Tax = bp.Tax
                    }).ToList();

                    Console.WriteLine($"Mapped {productData.Count} products for response");

                    return Json(new { success = true, data = productData });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in GetBillProducts for bill {billId}: {ex.Message}");
                    return Json(new { success = false, message = "An error occurred while fetching bill products." });
                }
            }



        public async Task<IActionResult> ExportAsPdf(Guid billId)
        {
            var bill = await _context.Bill
                .Include(b => b.BillProducts)
                    .ThenInclude(bp => bp.Product)
                        .ThenInclude(p => p.ProductTranslation)
                .FirstOrDefaultAsync(b => b.Id == billId);

            if (bill == null)
            {
                return NotFound();
            }

            using (var memoryStream = new MemoryStream())
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Verdana", 20, XFontStyle.Bold);

                gfx.DrawString("Bill Details", font, XBrushes.Black, new XRect(0, 0, page.Width, 50), XStringFormats.Center);

                font = new XFont("Verdana", 12, XFontStyle.Regular);
                gfx.DrawString($"Bill Date: {bill.BillDate.ToString("yyyy-MM-dd")}", font, XBrushes.Black, new XPoint(40, 100));
                gfx.DrawString($"Total Amount: {bill.TotalAmount}", font, XBrushes.Black, new XPoint(40, 130));
                gfx.DrawString($"Discount: {bill.Discount}", font, XBrushes.Black, new XPoint(40, 160));
                gfx.DrawString($"Tax: {bill.Tax}", font, XBrushes.Black, new XPoint(40, 190));
                gfx.DrawString($"Total Price: {bill.TotalPrice}", font, XBrushes.Black, new XPoint(40, 220));

                gfx.DrawString("Products:", font, XBrushes.Black, new XPoint(40, 260));
                var yPoint = 290;
                foreach (var item in bill.BillProducts)
                {
                    gfx.DrawString($"- {item.Product.ProductTranslation.FirstOrDefault(pt => pt.LangCode == "en-US")?.Name ?? "Unknown"}: {item.Quantity} x {item.Price} (Discount: {item.Discount}, Tax: {item.Tax})", font, XBrushes.Black, new XPoint(60, yPoint));
                    yPoint += 30;
                }

                document.Save(memoryStream);
                var fileContent = memoryStream.ToArray();
                return File(fileContent, "application/pdf", "BillDetails.pdf");
            }
        }

        public async Task<IActionResult> Create()
            {
                var products = await _context.Product
                                             .Include(p => p.ProductTranslation)
                                             .Where(p => p.ProductTranslation.Any(pt => pt.LangCode == "en-US"))
                                             .ToListAsync();

                var defaultProductId = products.FirstOrDefault()?.Id;

                ViewBag.Products = products.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.ProductTranslation.First(pt => pt.LangCode == "en-US").Name,
                    Selected = p.Id == defaultProductId
                }).ToList();

                ViewBag.ViewPath = "Ecommerce/_StoreBillEditor";
                return View(ViewBag.ViewPath);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(BillViewModel model)
            {
                if (ModelState.IsValid)
                {

                    var bill = new Bill
                    {
                        BillDate = model.BillDate,
                        Discount = model.Discount,
                        Tax = model.Tax,
                    };

                    _context.Bill.Add(bill);
                    await _context.SaveChangesAsync();

                    decimal totalAmount = 0;

                    foreach (var item in model.Items)
                    {
                        var billProduct = new BillProduct
                        {
                            BillID = bill.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Discount = item.Discount,
                            Tax = item.Tax
                        };
                        totalAmount += (item.Price * item.Quantity) - item.Discount + item.Tax;
                        _context.BillProduct.Add(billProduct);
                    }

                    bill.TotalAmount = totalAmount;
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }


            public async Task<IActionResult> Edit(Guid id)
            {
                var bill = await _context.Bill
                    .Include(b => b.BillProducts)
                        .ThenInclude(bp => bp.Product)
                            .ThenInclude(p => p.ProductTranslation)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (bill == null)
                {
                    return NotFound();
                }

                var viewModel = new BillEditViewModel
                {
                    Id = bill.Id,
                    BillDate = bill.BillDate,
                    TotalAmount = bill.TotalAmount,
                    Discount = bill.Discount,
                    Tax = bill.Tax,
                    TotalPrice = bill.TotalPrice,
                    Items = bill.BillProducts.Select(bp => new BillProductViewModel
                    {
                        ProductId = bp.ProductId,
                        ProductName = bp.Product.ProductTranslation.FirstOrDefault(pt => pt.LangCode == "en-US")?.Name ?? "Unknown",
                        Quantity = bp.Quantity,
                        Price = bp.Price,
                        Discount = bp.Discount,
                        Tax = bp.Tax
                    }).ToList()
                };

                await PopulateProductDropdownAsync();
                return View("CenterEmployeeReception/_Edit_Store_Bill", viewModel);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(Guid id, BillEditViewModel model)
            {
                if (id != model.Id)
                {
                    return BadRequest();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var bill = await _context.Bill
                            .Include(b => b.BillProducts)
                            .FirstOrDefaultAsync(b => b.Id == id);

                        if (bill == null)
                        {
                            return NotFound();
                        }

                        // Update bill properties
                        bill.BillDate = model.BillDate;
                        bill.Discount = model.Discount;
                        bill.Tax = model.Tax;

                        // Remove existing bill products
                        _context.BillProduct.RemoveRange(bill.BillProducts);

                        // Add updated bill products
                        foreach (var item in model.Items)
                        {
                            bill.BillProducts.Add(new BillProduct
                            {
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                Discount = item.Discount,
                                Tax = item.Tax
                            });
                        }

                        // Recalculate total amount
                        bill.TotalAmount = model.Items.Sum(i => i.Quantity * i.Price);

                        _context.Update(bill);
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = "Bill updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!await BillExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "The record has been modified by another user. Please refresh and try again.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, "An error occurred while saving the changes. Please try again.");
                        // Log the exception using Console.WriteLine
                        Console.WriteLine($"Error updating bill {id}: {ex.Message}");
                        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    }
                }

                // If we got this far, something failed, redisplay form
                await PopulateProductDropdownAsync();
                return View("CenterEmployeeReception/_Edit_Store_Bill", model);
            }

            private async Task PopulateProductDropdownAsync()
            {
                var products = await _context.Product
                    .Include(p => p.ProductTranslation)
                    .Where(p => p.ProductTranslation.Any(pt => pt.LangCode == "en-US"))
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.ProductTranslation.First(pt => pt.LangCode == "en-US").Name
                    })
                    .ToListAsync();

                ViewBag.Products = products;
            }

            private async Task<bool> BillExists(Guid id)
            {
                return await _context.Bill.AnyAsync(e => e.Id == id);
            }
    

    
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Delete([FromBody] DeleteBillModel model)
            {
                try
                {
                    var bill = await _context.Bill
                        .Include(b => b.BillProducts)
                        .FirstOrDefaultAsync(b => b.Id == model.Id);

                    if (bill == null)
                    {
                        return Json(new { success = false, message = "Bill not found" });
                    }

                    _context.BillProduct.RemoveRange(bill.BillProducts);
                    _context.Bill.Remove(bill);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            public class DeleteBillModel
            {
                public Guid Id { get; set; }
            }


        }
    }
