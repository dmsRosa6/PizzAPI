using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzAPI.Dtos;
using PizzAPI.Models;

namespace PizzAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly PizzaStoreContext _context;

        public EmployeeController(PizzaStoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.DeliveryDriver)
                .Include(e => e.StoreStaff)
                .Include(e => e.Store)
                .Where(e => e.IsActive)
                .ToListAsync();

            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.DeliveryDriver)
                .Include(e => e.StoreStaff)
                .Include(e => e.Store)
                .FirstOrDefaultAsync(e => e.EmployeeId == id && e.IsActive);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPost("store")]
        public async Task<IActionResult> CreateStoreEmployee(StoreEmployeeCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var employee = new Employee
                {
                    Name = request.Name,
                    Salary = request.Salary,
                    StoreId = request.StoreId,
                    IsActive = request.IsActive
                };

                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();

                var storeStaff = new StoreStaff
                {
                    EmployeeId = employee.EmployeeId,
                    Role = request.Role
                };

                await _context.StoreStaffs.AddAsync(storeStaff);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("driver")]
        public async Task<IActionResult> CreateDriverEmployee(DriverEmployeeCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var employee = new Employee
                {
                    Name = request.Name,
                    Salary = request.Salary,
                    StoreId = request.StoreId,
                    IsActive = request.IsActive
                };

                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();

                var driver = new DeliveryDriver
                {
                    EmployeeId = employee.EmployeeId,
                    Licence = request.Licence
                };

                await _context.DeliveryDrivers.AddAsync(driver);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("both")]
        public async Task<IActionResult> CreateCombinedEmployee(CombinedEmployeeCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var employee = new Employee
                {
                    Name = request.Name,
                    Salary = request.Salary,
                    StoreId = request.StoreId,
                    IsActive = request.IsActive
                };

                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(request.Role))
                {
                    var storeStaff = new StoreStaff
                    {
                        EmployeeId = employee.EmployeeId,
                        Role = request.Role
                    };

                    await _context.StoreStaffs.AddAsync(storeStaff);
                }

                if (!string.IsNullOrEmpty(request.Licence))
                {
                    var driver = new DeliveryDriver
                    {
                        EmployeeId = employee.EmployeeId,
                        Licence = request.Licence
                    };

                    await _context.DeliveryDrivers.AddAsync(driver);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("{id}/add-store-role")]
        public async Task<IActionResult> AddStoreStaffRole(int id, [FromBody] string role)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound("Employee not found.");

            var storeStaff = new StoreStaff
            {
                EmployeeId = id,
                Role = role
            };

            _context.StoreStaffs.Add(storeStaff);
            await _context.SaveChangesAsync();

            return Ok("Store staff role added.");
        }

        [HttpPost("{id}/add-driver-role")]
        public async Task<IActionResult> AddDriverRole(int id, [FromBody] string licence)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound("Employee not found.");

            var driver = new DeliveryDriver
            {
                EmployeeId = id,
                Licence = licence
            };

            _context.DeliveryDrivers.Add(driver);
            await _context.SaveChangesAsync();

            return Ok("Driver role added.");
        }

        [HttpDelete("{id}/remove-store-role")]
        public async Task<IActionResult> RemoveStoreStaffRole(int id)
        {
            var storeStaff = await _context.StoreStaffs.FindAsync(id);
            if (storeStaff == null) return NotFound("Store staff role not found.");

            _context.StoreStaffs.Remove(storeStaff);
            await _context.SaveChangesAsync();

            return Ok("Store staff role removed.");
        }

        [HttpDelete("{id}/remove-driver-role")]
        public async Task<IActionResult> RemoveDriverRole(int id)
        {
            var driver = await _context.DeliveryDrivers.FindAsync(id);
            if (driver == null) return NotFound("Driver role not found.");

            _context.DeliveryDrivers.Remove(driver);
            await _context.SaveChangesAsync();

            return Ok("Driver role removed.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee updatedEmployee)
        {
            if (id != updatedEmployee.EmployeeId)
                return BadRequest("Employee ID mismatch.");

            _context.Entry(updatedEmployee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Employees.Any(e => e.EmployeeId == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            if (!employee.IsActive)
            {
                return BadRequest();
            }

            employee.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
                return NotFound();

            if (employee.IsActive)
            {
                return BadRequest();
            }

            employee.IsActive = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
