using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzAPI.Models;

namespace PizzAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly PizzaStoreContext _context;

        public StoreController(PizzaStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Store>>> GetStores()
        {
            var stores = await _context.Stores
                .Include(s => s.Employees)
                .ToListAsync();

            return Ok(stores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Store>> GetStore(int id)
        {
            var store = await _context.Stores
                .Include(s => s.Employees)
                .FirstOrDefaultAsync(s => s.StoreId == id);

            if (store == null)
                return NotFound();

            return Ok(store);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStore(Store store)
        {
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStore), new { id = store.StoreId }, store);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStore(int id, Store updatedStore)
        {
            if (id != updatedStore.StoreId)
                return BadRequest("Store ID mismatch.");

            _context.Entry(updatedStore).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Stores.Any(s => s.StoreId == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }
    }
}
