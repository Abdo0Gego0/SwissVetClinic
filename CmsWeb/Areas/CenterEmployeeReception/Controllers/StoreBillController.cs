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
            var billProducts = await _context.BillProduct
                .Where(bp => bp.BillID == billId)
                .Include(bp => bp.Product)
                .ThenInclude(bp => bp.ProductTranslation)
                .ToListAsync();

            return PartialView("CenterEmployeeReception/_BillProductsPartial", billProducts);
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
            var bill = await Bill.GetFromDb(id, _context);
            if (bill == null)
            {
                return NotFound();
            }
            return View(bill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Bill bill)
        {
            if (id != bill.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Update(bill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await Bill.DeleteFromDb(id, _context);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await Bill.DeleteFromDb(id, _context);
            return RedirectToAction(nameof(Index));
        }
    }
}
