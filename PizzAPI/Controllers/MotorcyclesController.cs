using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzAPI.Models;

namespace PizzAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MotorcyclesController : ControllerBase
{
    private readonly PizzaStoreContext _context;

    public MotorcyclesController(PizzaStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Motorcycle>>> GetMotorcycles()
    {
        return await _context.Motorcycles
            .Where(m => !m.IsDeleted)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Motorcycle>> GetMotorcycle(int id)
    {
        var motorcycle = await _context.Motorcycles
            .FirstOrDefaultAsync(m => m.MotorcycleId == id && !m.IsDeleted);

        if (motorcycle == null)
        {
            return NotFound();
        }

        return motorcycle;
    }

    [HttpPost]
    public async Task<ActionResult<Motorcycle>> CreateMotorcycle(Motorcycle motorcycle)
    {
        _context.Motorcycles.Add(motorcycle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMotorcycle), new { id = motorcycle.MotorcycleId }, motorcycle);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> SoftDeleteMotorcycle(int id)
    {
        var motorcycle = await _context.Motorcycles.FindAsync(id);
        if (motorcycle == null || motorcycle.IsDeleted)
        {
            return NotFound();
        }

        motorcycle.IsDeleted = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
