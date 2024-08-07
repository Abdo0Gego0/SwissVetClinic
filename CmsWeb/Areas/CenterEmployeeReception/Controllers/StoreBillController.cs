using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CmsDataAccess.DbModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using CmsWeb.Areas.CenterEmployeeReception.Models;

using CmsDataAccess.MobileViewModels.AuthModels;

namespace YourAppName.Controllers
{
    [Area("CenterEmployeeReception")]
    public class StoreBillController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StoreBillController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var bills = await _context.Bill.ToListAsync();
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

                if (billProducts == null || !billProducts.Any())
                {
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

                return Json(new { success = true, data = productData });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in GetBillProducts: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while fetching bill products." });
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
                TotalAmount = bill.TotalAmount ?? 0,
                Discount = bill.Discount ?? 0,
                Tax = bill.Tax ?? 0,
                TotalPrice = bill.TotalPrice ?? 0,
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
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BillExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Refresh the data and show a message to the user
                        ModelState.AddModelError(string.Empty, "The record has been modified by another user. The edit operation was canceled and the current values in the database have been displayed. If you still want to edit this record, click the Save button again.");
                        await PopulateProductDropdownAsync();
                        return View(model);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateProductDropdownAsync();
            return RedirectToAction(nameof(Index));
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
