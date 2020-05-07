using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Interact with the book table
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : BookStoreBaseController
    {
        private readonly IBookRepository _bookRepository;
        public BooksController(IBookRepository bookRepository,
            ILoggerService logger,
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all Book
        /// </summary>
        /// <returns>List of Books</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            var location = GetControllerActionName();
            try
            {
                _logger.LogInfo($"{location} : Attemped get all Bookss");
                var books = await _bookRepository.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo($"{location} : Successfully get all Books");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location} : {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get a book by Id
        /// </summary>
        /// <param> name="id"</param>
        /// <returns>a book</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)
        {
            var location = GetControllerActionName();
            try
            {
                _logger.LogInfo($"{location} : Attemped get a book:{id}");
                var book = await _bookRepository.FindById(id);
                if (book == null)
                {
                    _logger.LogWarn($"{location} : Book:{id} was not found");
                    return NotFound();
                }
                var response = _mapper.Map<BookDTO>(book);
                _logger.LogInfo($"{location} :Successfully get a book:{id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location} : {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Create a book
        /// </summary>
        /// <param> name="BookDTO"></param>
        /// <returns></returns>
        /// 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO)
        {
            var location = GetControllerActionName();
            try
            {
                if (bookDTO == null)
                {
                    _logger.LogWarn($"{location} : Emtpy request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location} : Modelstate was incomplete");
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _bookRepository.Create(book);
                if (!isSuccess)
                {
                    return InternalError($"{location} : Book Create failed");
                }
                _logger.LogInfo($"{location} : Book Created");
                return Created("Create", new { book });
            }
            catch (Exception e)
            {
                return InternalError($"{location} : {e.Message} - {e.InnerException}");
            }

        }

        /// <summary>
        /// Update a book
        /// </summary>
        /// <param> name="id"></param>
        /// <param> name="Book"></param>
        /// <returns></returns>
        /// 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int Id, [FromBody] BookUpdateDTO bookDTO)
        {
            var location = GetControllerActionName();
            try
            {
                _logger.LogInfo($"{location} : Attempt Book Update");
                if (Id < 1 || bookDTO == null || Id != bookDTO.Id)
                {
                    _logger.LogWarn($"{location} : Emtpy request was submitted");
                    return BadRequest();
                }
                var Exists = await _bookRepository.Exists(Id);
                if (!Exists)
                {
                    return NotFound();
                }
                var author = await _bookRepository.FindById(Id);
                var isSuccess = await _bookRepository.Update(author);
                if (!isSuccess)
                {
                    return InternalError($"{location} : book Update  failed");
                }
                _logger.LogInfo($"{location} : book Updated");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }

        }

        /// <summary>
        /// Delete a book
        /// </summary>
        /// <param> name="id"></param>
        /// <returns></returns>
        /// 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            var location = GetControllerActionName();
            try
            {
                _logger.LogInfo($"{location} : Attempt Book Delete");
                if (Id < 1)
                {
                    _logger.LogWarn($"{location} :  id < 1 was submitted");
                    return BadRequest();
                }
                var author = await _bookRepository.FindById(Id);
                if (author == null)
                {
                    return NotFound();
                }
                var isSuccess = await _bookRepository.Delete(author);
                if (!isSuccess)
                {
                    return InternalError($"{location} :  Book Delete failed");
                }
                _logger.LogInfo($"{location} : Book Deleted");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }

        }

        
    }
}