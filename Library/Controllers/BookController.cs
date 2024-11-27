using Library.Application.Contracts.Books;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IWebHostEnvironment _env;

        public BooksController(IBookService bookService, IWebHostEnvironment env)
        {
            _bookService = bookService;
            _env = env;
        }   

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, string? search = null)
        {
            var books = await _bookService.GetAllAsync(page, pageSize, search);
            return Ok(books);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) return NotFound();

            var response = new GetBookResponse
            {
                Id = book.Id,
                ISBN = book.ISBN,
                Title = book.Title,
                Genre = book.Genre,
                Description = book.Description,
                AuthorId = book.AuthorId,
                BorrowedOn = book.BorrowedOn,
                ReturnBy = book.ReturnBy,
                ImagePath = book.ImagePath
            };

            return Ok(response);
        }

        [HttpGet("isbn/{isbn}")]
        public async Task<IActionResult> GetByISBN(string isbn)
        {
            var book = await _bookService.GetByISBNAsync(isbn);
            if (book == null) return NotFound();

            var response = new GetBookResponse
            {
                Id = book.Id,
                ISBN = book.ISBN,
                Title = book.Title,
                Genre = book.Genre,
                Description = book.Description,
                AuthorId = book.AuthorId,
                BorrowedOn = book.BorrowedOn,
                ReturnBy = book.ReturnBy,
                ImagePath = book.ImagePath
            };

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateBookRequest request)
        {
            var bookDto = new BookDTO
            {
                Id = Guid.NewGuid(),
                ISBN = request.ISBN,
                Title = request.Title,
                Genre = request.Genre,
                Description = request.Description,
                AuthorId = request.AuthorId,
                BorrowedOn = request.BorrowedOn,
                ReturnBy = request.ReturnBy,
                ImagePath = request.ImagePath
            };
            await _bookService.AddAsync(bookDto);
            return CreatedAtAction(nameof(GetById), new { id = bookDto.Id }, bookDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookRequest request)
        {
            var bookDto = new BookDTO
            {
                Id = id,
                ISBN = request.ISBN,
                Title = request.Title,
                Genre = request.Genre,
                Description = request.Description,
                AuthorId = request.AuthorId,
                BorrowedOn = request.BorrowedOn,
                ReturnBy = request.ReturnBy,
                ImagePath = request.ImagePath
            };
            await _bookService.UpdateAsync(id, bookDto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _bookService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id:guid}/borrow")]
        public async Task<IActionResult> BorrowBook(Guid id, [FromBody] BorrowBookRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _bookService.BorrowBookAsync(id, Guid.Parse(userId), request.ReturnBy);
            return Ok();
        }
    }
}