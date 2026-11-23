using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterApi.Models;
using ShelterApi.Models.DTOs;

namespace ShelterApi.Controllers;

[ApiController]
[Route("shelters")]
public class SheltersController : ApiControllerBase
{
    private readonly AnimalshelterContext _context;

    public SheltersController(AnimalshelterContext context)
    {
        _context = context;
    }

    private static string? Validate(string? name, string? city, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name)) return "A név megadása kötelező.";
        if (name.Length > 50) return "A név legfeljebb 50 karakter lehet.";
        if (string.IsNullOrWhiteSpace(city)) return "A város megadása kötelező.";
        if (city.Length > 40) return "A város legfeljebb 40 karakter lehet.";
        if (capacity < 0) return "A kapacitás nem lehet negatív.";
        return null;
    }

    [HttpPost("newShelter")]
    public async Task<IActionResult> NewShelter([FromBody] NewShelterDto dto)
    {
        try
        {
            var error = Validate(dto?.Name, dto?.City, dto?.Capacity ?? 0);
            if (dto is null || error is not null)
                return Respond(400, error ?? "Hiányzó kérés törzs.");

            var shelter = new Shelter
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                City = dto.City.Trim(),
                Capacity = dto.Capacity,
                RegTime = DateTime.Now
            };

            _context.Shelters.Add(shelter);
            await _context.SaveChangesAsync();

            return Respond(201, "A menhely sikeresen rögzítve.", shelter);
        }
        catch (Exception ex)
        {
            return ServerError(ex);
        }
    }

    [HttpGet("allShelter")]
    public async Task<IActionResult> AllShelter()
    {
        try
        {
            var shelters = await _context.Shelters.ToListAsync();
            if (shelters.Count == 0)
                return Respond(404, "Nincs menhely az adatbázisban.");

            return Respond(200, "A menhelyek listája.", shelters);
        }
        catch (Exception ex)
        {
            return ServerError(ex);
        }
    }

    [HttpGet("byId")]
    public async Task<IActionResult> ById([FromQuery] Guid id)
    {
        try
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(s => s.Id == id);
            if (shelter is null)
                return Respond(404, "Nem található menhely ezzel az azonosítóval.");

            return Respond(200, "A menhely adatai.", shelter);
        }
        catch (Exception ex)
        {
            return ServerError(ex);
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromQuery] Guid id, [FromBody] UpdateShelterDto dto)
    {
        try
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(s => s.Id == id);
            if (shelter is null)
                return Respond(404, "Nem található menhely ezzel az azonosítóval.");

            var error = Validate(dto?.Name, dto?.City, dto?.Capacity ?? 0);
            if (dto is null || error is not null)
                return Respond(400, error ?? "Hiányzó kérés törzs.");

            shelter.Name = dto.Name.Trim();
            shelter.City = dto.City.Trim();
            shelter.Capacity = dto.Capacity;
            await _context.SaveChangesAsync();

            return Respond(200, "A menhely sikeresen módosítva.", shelter);
        }
        catch (Exception ex)
        {
            return ServerError(ex);
        }
    }

    [HttpDelete("deleteShelter")]
    public async Task<IActionResult> DeleteShelter([FromQuery] Guid id)
    {
        try
        {
            var shelter = await _context.Shelters.FirstOrDefaultAsync(s => s.Id == id);
            if (shelter is null)
                return Respond(404, "Nem található menhely ezzel az azonosítóval.");

            _context.Shelters.Remove(shelter);
            await _context.SaveChangesAsync();

            return Respond(200, "A menhely (és a hozzá tartozó állatok) sikeresen törölve.");
        }
        catch (Exception ex)
        {
            return ServerError(ex);
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
                return Respond(404, "Nincs menhely az adatbázisban.");

            return Respond(200, "A menhelyek az állataikkal.", shelters);
        }
        catch (Exception ex)
        {
            return ServerError(ex);
        }
    }
}