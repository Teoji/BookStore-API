using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with the authors
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : BookStoreBaseController
    {
        private readonly IAuthorRepository _authorRepository;
        /*private readonly ILoggerService _logger;
        private readonly IMapper _mapper;*/
        public AuthorsController(IAuthorRepository authorRepository, 
            ILoggerService logger,
            IMapper mapper) : base()
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all Authors
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet]
        [Authorize(Roles="Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            var location = GetControllerActionName();
            try
            {
                _logger.LogInfo($"{location} : Attemped get all Authors");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo($"{location} : Successfully get all Authors");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location} : {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get an Author by Id
        /// </summary>
        /// <param> name="id"</param>
        /// <returns>an Author</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            var location = GetControllerActionName();
            try
            {
                _logger.LogInfo($"{location} : Attemped get an Author:{id}");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"{location} : Author:{id} was not found");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"{location} : Successfully get an Author:{id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location} : {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Create an Author
        /// </summary>
        /// <param> name="AuthorDTO"></param>
        /// <returns></returns>
        /// 
        
        [HttpPost]
        [Authorize (Roles="Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                if (authorDTO == null)
                {
                    _logger.LogWarn("Emtpy request was submitted");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn("Modelstate was incomplete");
                    return BadRequest(ModelState);
                }
                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);
                if (!isSuccess)
                {
                    return InternalError($"Author Create failed");
                }
                _logger.LogInfo("Author Created");
                return Created("Create", new { author});
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        
        }

        /// <summary>
        /// Update an Author
        /// </summary>
        /// <param> name="id"></param>
        /// <param> name="Author"></param>
        /// <returns></returns>
        /// 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int Id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                _logger.LogInfo("Attempt Author Update");
                if (Id < 1 || authorDTO == null || Id != authorDTO.Id)
                {
                    _logger.LogWarn("Emtpy request was submitted");
                    return BadRequest();
                }
                var Exists = await _authorRepository.isExists(Id);
                if (!Exists)
                {
                    return NotFound();
                }
                var author = await _authorRepository.FindById(Id);
                var isSuccess = await _authorRepository.Update(author);
                if (!isSuccess)
                {
                    return InternalError($"Author Update  failed");
                }
                _logger.LogInfo("Author Updated");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }

        }

        /// <summary>
        /// Delete an Author
        /// </summary>
        /// <param> name="id"></param>
        /// <returns></returns>
        /// 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Delete(int Id)
        {
            try
            {
                _logger.LogInfo("Attempt Author Delete");
                if (Id < 1)
                {
                    _logger.LogWarn("id < 1 was submitted");
                    return BadRequest();
                }
                var author = await _authorRepository.FindById(Id);
                if (author == null)
                {
                    return NotFound();
                }
                var isSuccess = await _authorRepository.Delete(author);
                if (!isSuccess)
                {
                    return InternalError($"Author Delete failed");
                }
                _logger.LogInfo("Author Deleted");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }

        }

        /*private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the Admin");
        }*/
    }
}