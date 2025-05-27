using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzAPI.Models;

namespace PizzAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PizzaController : Controller
    {

        private readonly PizzaStoreContext _context;

        public PizzaController(PizzaStoreContext pizzaStoreContext)
        {
            this._context = pizzaStoreContext;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pizza>> GetPizzaById(int id)
        {
            var pizza = await this._context.Pizzas.FindAsync(id);

            if (pizza == null || pizza.IsDeleted == true)
            {
                return NotFound();
            }

            return pizza;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pizza>>> GetPizzas()
        {
            var pizzas = await _context.Pizzas.Where<Pizza>(p => p.IsDeleted == false).ToListAsync();

            return pizzas;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePizza(Pizza pizza)
        {

            await _context.AddAsync(pizza);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Maybe there is a cleaner way of doin this
                throw;
            }

            return CreatedAtAction(nameof(GetPizzaById), new { id = pizza.PizzaId }, pizza);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePizzaById(int id)
        {
            var pizza = await _context.Pizzas.FindAsync(id);

            if (pizza == null)
                return NotFound();

            if (pizza.IsDeleted)
                return BadRequest();

            pizza.IsDeleted = true;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> UndoDeletePizzaById(int id)
        {
            var pizza = await _context.Pizzas.FindAsync(id);

            if (pizza == null)
                return NotFound();

            if (!pizza.IsDeleted)
                return BadRequest();

            pizza.IsDeleted = false;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePizza(int id, Pizza updatedPizza)
        {
            if (id != updatedPizza.PizzaId)
            {
                return BadRequest("Pizza ID in URL does not match body.");
            }

            _context.Entry(updatedPizza).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //TODO
                throw;
            }

            return NoContent();
        }

    }
}