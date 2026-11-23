using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterApi.Models;
using ShelterApi.Models.DTOs;

namespace ShelterApi.Controllers;

[ApiController]
[Route("shelters")]
public class SheltersController : ControllerBase
{
    private readonly AnimalshelterContext _context;

    public SheltersController(AnimalshelterContext context)
    {
        _context = context;
    }

    [HttpPost("newShelter")]
    public async Task<IActionResult> NewShelter([FromBody] NewShelterDto dto)
    {
        try
        {
            var shelter = new Shelter
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                City = dto.City,
                Capacity = dto.Capacity,
                RegTime = DateTime.Now
            };

            _context.Shelters.Add(shelter);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                Message = "A menhely sikeresen rögzítve.",
                Data = shelter
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("allShelter")]
    public async Task<IActionResult> AllShelter()
    {
        try
        {
            var shelters = await _context.Shelters.ToListAsync();
            if (shelters.Count == 0)
                return NotFound("Nincs menhely az adatbázisban.");

            return Ok(new
            {
                Message = "A menhelyek listája.",
                Data = shelters
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("byId")]
    public async Task<IActionResult> ById([FromQuery] Guid id)
    {
        try
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(s => s.Id == id);
            if (shelter is null)
                return NotFound("Nem található menhely ezzel az azonosítóval.");

            return Ok(new
            {
                Message = "A menhely adatai.",
                Data = shelter
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromQuery] Guid id, [FromBody] UpdateShelterDto dto)
    {
        try
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(s => s.Id == id);
            if (shelter is null)
                return NotFound("Nem található menhely ezzel az azonosítóval.");

            shelter.Name = dto.Name;
            shelter.City = dto.City;
            shelter.Capacity = dto.Capacity;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "A menhely sikeresen módosítva.",
                Data = shelter
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("deleteShelter")]
    public async Task<IActionResult> DeleteShelter([FromQuery] Guid id)
    {
        try
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(s => s.Id == id);
            if (shelter is null)
                return NotFound("Nem található menhely ezzel az azonosítóval.");

            _context.Shelters.Remove(shelter);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "A menhely (és a hozzá tartozó állatok) sikeresen törölve."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("allShelterWithAnimals")]
    public async Task<IActionResult> AllShelterWithAnimals()
    {
        try
        {
            var shelters = await _context.Shelters
                .Include(s => s.Animals)
                .ToListAsync();

            if (shelters.Count == 0)
                return NotFound("Nincs menhely az adatbázisban.");

            return Ok(new
            {
                Message = "A menhelyek az állataikkal.",
                Data = shelters
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}   