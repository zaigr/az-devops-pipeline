using System;
using Microsoft.AspNetCore.Mvc;

namespace Production.Api.Exceptions.Handlers
{
    public class EntryNotFoundExceptionHandler : IExceptionHandler
    {
        public ObjectResult Handle(Exception exception)
        {
            if (!(exception is EntryNotFoundException notFoundException))
            {
                return null;
            }

            return new NotFoundObjectResult(notFoundException.Value);
        }
    }
}
