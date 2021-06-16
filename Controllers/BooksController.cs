using BookAPI.Models;
using BookAPI.Repositories;
using ImageUploader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _repository;

        public BooksController(IBookRepository bookRepository)
        {
            _repository = bookRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Book>> GetBooks()
        {
            return await _repository.Get();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBooks(int id)
        {
            return await _repository.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<Book>>PostBooks([FromBody] Book book)
        {
            var newBook = await _repository.Create(book);
            return CreatedAtAction(nameof(GetBooks), new { id = newBook.Id }, newBook);
            
           
            
                var stream = new MemoryStream(book.ImageArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot";
                var response = FilesHelper.UploadImage(stream, folder, file);
                if (!response)
                {
                    return BadRequest();
                }
                else
                {
                    book.ImageUrl = file;
                    await _repository.Create(book);
                    
                    return StatusCode(StatusCodes.Status201Created);
                }
          
        }

        [HttpPut]
        public async Task<ActionResult> PutBooks(int id, [FromBody] Book book)
        {
            if(id != book.Id)
            {
                return BadRequest();
            }

            await _repository.Update(book);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete (int id)
        {
            var bookToDelete = await _repository.Get(id);
            if (bookToDelete == null)
                return NotFound();

            await _repository.Delete(bookToDelete.Id);
            return NoContent();
        }
    }
}
