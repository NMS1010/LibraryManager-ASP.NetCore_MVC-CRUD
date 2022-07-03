using BookManagerMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace BookManagerMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly Service _service;
        public BookController(Service service)
        {
            _service = service;
        }
        public IActionResult Index(string orderBy = "Name", bool asc = true, int currPage = 1)
        {
            var res = _service.Pagination(orderBy, asc, currPage);
            ViewData["CurrPage"] = res.currPage;
            ViewData["TotalPages"] = res.totalPages;

            ViewData["Name"] = true;
            ViewData["Authors"] = true;
            ViewData["Year"] = true;
            ViewData["Publisher"] = true;
            ViewData[orderBy] = !asc;

            return View(res.books);
        }
        public IActionResult Details(int id)
        {
            Book book = _service.GetBook(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        public IActionResult Delete(int id)
        {
            Book book = _service.GetBook(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        public IActionResult Delete(Book book)
        {
            _service.DeleteBook(book.Id);
            _service.SaveChange();
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            Book book = _service.GetBook(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        public IActionResult Update(Book book, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                _service.Upload(book, file);
                _service.UpdateBook(book);
                _service.SaveChange();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        public IActionResult Create()
        {
            Book book = _service.CreateBook();
            return View(book);
        }

        [HttpPost]
        public IActionResult Create(Book book, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                _service.Upload(book, file);
                _service.AddBook(book);
                _service.SaveChange();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        public IActionResult Download(int id)
        {
            Book book = _service.GetBook(id);
            if (book == null) 
                return NotFound();
            if(!System.IO.File.Exists(_service.GetPathFile(book.FileName))) 
                return NotFound();
            var (memory, type) = _service.Download(book);
            return File(memory, type, book.FileName);
        }
        
        public IActionResult Search(string strSearch)
        {
            return View("Index",_service.Search(strSearch));
        }
    }
}
