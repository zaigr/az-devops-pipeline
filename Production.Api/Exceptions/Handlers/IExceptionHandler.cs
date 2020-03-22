using System;
using Microsoft.AspNetCore.Mvc;

namespace Production.Api.Exceptions.Handlers
{
    public interface IExceptionHandler
    {
        ObjectResult Handle(Exception exception);
    }
}
