using Microsoft.AspNetCore.Mvc;

namespace ShelterApi.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected ObjectResult ServerError(Exception ex)
        => StatusCode(500, new { message = "Szerverhiba történt: " + ex.Message });
}