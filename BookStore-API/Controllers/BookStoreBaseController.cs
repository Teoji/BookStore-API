using AutoMapper;
using BookStore_API.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    public class BookStoreBaseController : ControllerBase
    {
        protected ILoggerService _logger;
        protected IMapper _mapper;
        public BookStoreBaseController() : base ()
        {

        }

        protected virtual string GetControllerActionName()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}";
        }

        protected virtual ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the Admin");
        }
    }
}
