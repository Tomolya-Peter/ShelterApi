using Microsoft.AspNetCore.Mvc;

namespace ShelterApi.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected ObjectResult Respond(int statusCode, string message, object? result = null)
        => StatusCode(statusCode, new { message, result });

    protected ObjectResult ServerError(Exception ex)
        => Respond(500, "Szerverhiba történt: " + ex.Message);
}