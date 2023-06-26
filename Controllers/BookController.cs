using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTBook.Models;
using FPTBook.Areas.Identity.Data;
using Microsoft.Extensions.Caching.Memory;
using FPTBook.Utils;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Data.SqlClient;


namespace FPTBook.Controllers
{

    public class BookController : Controller
    {
        private readonly FPTBookIdentityDbContext _context;
        private readonly IWebHostEnvironment hostEnvironment;

        public BookController(FPTBookIdentityDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            hostEnvironment = environment;
        }

        // GET: Book
        public async Task<IActionResult> Index(string searchString)
        {

            var dbook = from m in _context.Book
                        select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                dbook = dbook.Where(s => s.Name!.Contains(searchString));
            }
            // var fPTContext = _context.Book.Include(b => b.Author).Include(b => b.Category).Include(b => b.Publisher);
            return View(await dbook.ToListAsync());
        }
        [Authorize(Roles = "StoreOwner")]

        public async Task<IActionResult> Index1()
        {
            var fPTContext = _context.Book.Include(b => b.Author).Include(b => b.Category).Include(b => b.Publisher);
            return View(await fPTContext.ToListAsync());
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            ViewData["AuthorID"] = new SelectList(_context.Author, "Id", "Name");
            ViewData["CategoryID"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["PublisherID"] = new SelectList(_context.Publisher, "Id", "Name");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,UploadImage,AuthorID,CategoryID,PublisherID")] Book book, IFormFile myfile)
        {
            if (ModelState.IsValid)
            {
                string filename = Path.GetFileName(myfile.FileName);
                Console.WriteLine("_________________________________");
                Console.WriteLine(filename);
                var filePath = Path.Combine(hostEnvironment.WebRootPath, "uploads");
                string fullPath = filePath + "\\" + filename;
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await myfile.CopyToAsync(stream);
                }
                book.UploadImage = filename;
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["AuthorID"] = new SelectList(_context.Author, "Id", "Id", book.AuthorID);
                ViewData["CategoryID"] = new SelectList(_context.Category, "Id", "Id", book.CategoryID);
            ViewData["PublisherID"] = new SelectList(_context.Publisher, "Id", "Id", book.PublisherID);
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,UploadImage,AuthorID,CategoryID,PublisherID")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            ViewData["AuthorID"] = new SelectList(_context.Author, "Id", "Id", book.AuthorID);
            ViewData["CategoryID"] = new SelectList(_context.Category, "Id", "Id", book.CategoryID);
            ViewData["PublisherID"] = new SelectList(_context.Publisher, "Id", "Id", book.PublisherID);
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'FPTBookIdentityDbContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [HttpPost]
        public IActionResult AddToCart(int id, string name, double price, int quantity)
        {
            ShoppingCart myCart;

            if (HttpContext.Session.GetObject<ShoppingCart>("cart") == null)
            {
                myCart = new ShoppingCart();
                HttpContext.Session.SetObject("cart", myCart);
            }
            myCart = (ShoppingCart)HttpContext.Session.GetObject<ShoppingCart>("cart");
            var newItem = myCart.AddItem(id, name, price, quantity);
            HttpContext.Session.SetObject("cart", myCart);
            ViewData["newItem"] = newItem;
            return View();
        }
        public IActionResult CheckOut()
        {
            try
            {
                ShoppingCart cart = (ShoppingCart)HttpContext.Session.GetObject<ShoppingCart>("cart");
                ViewData["myItems"] = cart.Items;
                return View();
            }
            catch
            {

                return RedirectToAction("", "Book");
            }
        }
        public RedirectToActionResult EditOrder(int id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)HttpContext.Session.GetObject<ShoppingCart>("cart");
            cart.EditItem(id, quantity);
            HttpContext.Session.SetObject("cart", cart);

            return RedirectToAction("CheckOut", "Book");
        }
        [HttpPost]
        public RedirectToActionResult RemoveOrderItem(int id)
        {
            ShoppingCart cart = (ShoppingCart)HttpContext.Session.GetObject<ShoppingCart>("cart");
            cart.RemoveItem(id);
            HttpContext.Session.SetObject("cart", cart);

            return RedirectToAction("CheckOut", "Book");
        }

    }
}
