using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzAPI.Dtos;
using PizzAPI.Models;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using Microsoft.EntityFrameworkCore.Internal;

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


        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetClient(int id)
        {

            Client? client = await _context.Clients
                .Include(c => c.Address)
                    .ThenInclude(a => a.Municipality)
                        .ThenInclude(m => m.District)
                .FirstOrDefaultAsync(c => c.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(ClientDto.FromEntity(client));
        }

        [HttpPost()]
        public async Task<IActionResult> CreateClient(ClientCreateRequestDto clientDto)
        {
            if (clientDto.Address == null)
                return BadRequest("Address is required.");

            // The check is done by comparing the street and what not aswell as checking coordinates
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var inputPoint = geometryFactory.CreatePoint(new Coordinate(clientDto.Address.Longitude, clientDto.Address.Latitude));
            double maxDistanceMeters = 20;

            var existingAddress = await _context.Addresses
                    .Where(a =>
                    a.StreetName == clientDto.Address.StreetName &&
                    a.PostalCode == clientDto.Address.PostalCode &&
                    a.DoorNumber == clientDto.Address.DoorNumber &&
                    a.MunicipalityId == clientDto.Address.MunicipalityId &&
                    a.Geom.IsWithinDistance(inputPoint, maxDistanceMeters)
                ).FirstOrDefaultAsync();

            Address addressToUse;

            if (existingAddress != null)
            {
                addressToUse = existingAddress;
            }
            else
            {
                addressToUse = clientDto.Address.ToEntity();
                await _context.Addresses.AddAsync(addressToUse);
                await _context.SaveChangesAsync();
            }

            var client = new Client
            {
                Name = clientDto.Name,
                PhoneNumber = clientDto.PhoneNumber,
                Nif = clientDto.Nif,
                AddressId = addressToUse.AddressId
            };

            await _context.Clients.AddAsync(client);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("A concurrency conflict occurred.");
            }

            return CreatedAtAction(nameof(GetClient), new { id = client.ClientId }, ClientDto.FromEntity(client));
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
