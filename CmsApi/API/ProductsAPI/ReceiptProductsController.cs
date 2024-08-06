//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using CmsDataAccess.DbModels;

//namespace CmsApi.API.ProductsAPI
//{
//    public class ReceiptProductsController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public ReceiptProductsController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: ReceiptProducts
//        public async Task<IActionResult> Index()
//        {
//           // var applicationDbContext = _context.ReceiptProduct.Include(r => r.Product).Include(r => r.Receipt);
//           // return View(await applicationDbContext.ToListAsync());
//        }

//        // GET: ReceiptProducts/Details/5
//        public async Task<IActionResult> Details(Guid? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var receiptProduct = await _context.ReceiptProduct
//                .Include(r => r.Product)
//                .Include(r => r.Receipt)
//                .FirstOrDefaultAsync(m => m.ReceiptProductID == id);
//            if (receiptProduct == null)
//            {
//                return NotFound();
//            }

//            return View(receiptProduct);
//        }

//        // GET: ReceiptProducts/Create
//        public IActionResult Create()
//        {
//            ViewData["ProductID"] = new SelectList(_context.Product, "Id", "Id");
//            ViewData["ReceiptID"] = new SelectList(_context.Receipt, "ReceiptID", "ReceiptID");
//            return View();
//        }

//        // POST: ReceiptProducts/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("ReceiptProductID,ReceiptID,ProductID,Quantity,Price")] ReceiptProduct receiptProduct)
//        {
//            if (ModelState.IsValid)
//            {
//                receiptProduct.ReceiptProductID = Guid.NewGuid();
//                _context.Add(receiptProduct);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["ProductID"] = new SelectList(_context.Product, "Id", "Id", receiptProduct.ProductID);
//            ViewData["ReceiptID"] = new SelectList(_context.Receipt, "ReceiptID", "ReceiptID", receiptProduct.ReceiptID);
//            return View(receiptProduct);
//        }

//        // GET: ReceiptProducts/Edit/5
//        public async Task<IActionResult> Edit(Guid? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var receiptProduct = await _context.ReceiptProduct.FindAsync(id);
//            if (receiptProduct == null)
//            {
//                return NotFound();
//            }
//            ViewData["ProductID"] = new SelectList(_context.Product, "Id", "Id", receiptProduct.ProductID);
//            ViewData["ReceiptID"] = new SelectList(_context.Receipt, "ReceiptID", "ReceiptID", receiptProduct.ReceiptID);
//            return View(receiptProduct);
//        }

//        // POST: ReceiptProducts/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(Guid id, [Bind("ReceiptProductID,ReceiptID,ProductID,Quantity,Price")] ReceiptProduct receiptProduct)
//        {
//            if (id != receiptProduct.ReceiptProductID)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(receiptProduct);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!ReceiptProductExists(receiptProduct.ReceiptProductID))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["ProductID"] = new SelectList(_context.Product, "Id", "Id", receiptProduct.ProductID);
//            ViewData["ReceiptID"] = new SelectList(_context.Receipt, "ReceiptID", "ReceiptID", receiptProduct.ReceiptID);
//            return View(receiptProduct);
//        }

//        // GET: ReceiptProducts/Delete/5
//        public async Task<IActionResult> Delete(Guid? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var receiptProduct = await _context.ReceiptProduct
//                .Include(r => r.Product)
//                .Include(r => r.Receipt)
//                .FirstOrDefaultAsync(m => m.ReceiptProductID == id);
//            if (receiptProduct == null)
//            {
//                return NotFound();
//            }

//            return View(receiptProduct);
//        }

//        // POST: ReceiptProducts/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(Guid id)
//        {
//            var receiptProduct = await _context.ReceiptProduct.FindAsync(id);
//            if (receiptProduct != null)
//            {
//                _context.ReceiptProduct.Remove(receiptProduct);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool ReceiptProductExists(Guid id)
//        {
//            return _context.ReceiptProduct.Any(e => e.ReceiptProductID == id);
//        }
//    }
//}
