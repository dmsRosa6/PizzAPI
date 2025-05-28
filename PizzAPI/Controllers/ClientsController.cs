using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzAPI.Models;

namespace PizzAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly PizzaStoreContext _context;

        public ClientController(PizzaStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            var clients = await _context.Clients
                .ToListAsync();

            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient(Client client)
        {

            await _context.Clients.AddAsync(client);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DBConcurrencyException)
            {
                
            }

            return CreatedAtAction(nameof(GetClient), new { id = client.ClientId }, client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(int id, Client updatedClient)
        {
            if (id != updatedClient.ClientId)
                return BadRequest("Client ID mismatch.");

            _context.Entry(updatedClient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Clients.Any(c => c.ClientId == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

    }
}
