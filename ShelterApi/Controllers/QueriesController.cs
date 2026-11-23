using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterApi.Models;

namespace ShelterApi.Controllers;

[ApiController]
[Route("queries")]
public class QueriesController : ControllerBase
{
    private readonly AnimalshelterContext _context;

    public QueriesController(AnimalshelterContext context)
    {
        _context = context;
    }

    [HttpGet("animalsByShelter")]
    public async Task<IActionResult> AnimalsByShelter([FromQuery] Guid shelterId)
    {
        try
        {
            var animals = await _context.Animals
                .Where(a => a.ShelterId == shelterId)
                .OrderBy(a => a.Name)
                .ToListAsync();

            return Ok(new
            {
                Message = "A menhely állatai név szerint rendezve.",
                Data = animals
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("animalStats")]
    public async Task<IActionResult> AnimalStats()
    {
        try
        {
            var count = await _context.Animals.CountAsync();
            if (count == 0)
                return NotFound("Nincs állat az adatbázisban.");

            var result = new
            {
                Count = count,
                MinWeight = await _context.Animals.MinAsync(a => a.Weight),
                MaxWeight = await _context.Animals.MaxAsync(a => a.Weight),
                AverageWeight = await _context.Animals.AverageAsync(a => a.Weight),
                TotalWeight = await _context.Animals.SumAsync(a => a.Weight)
            };

            return Ok(new
            {
                Message = "Az állatok testtömegének statisztikája",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}