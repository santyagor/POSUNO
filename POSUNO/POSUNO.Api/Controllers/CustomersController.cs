using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSUNO.Api.Data.Entities;
using POSUNO.Api.Models;

namespace POSUNO.Api.Controllers
{
    
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly DataContext _context;

        public CustomersController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, CustomerRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return BadRequest("Usuario no existe");
            }

            Customer customer = await _context.Customers.FindAsync(id);
            if(customer == null)
            {
                return BadRequest("Cliente no existe.");
            }
            customer.FirtsName = request.FirtsName;
            customer.LastName = request.LastName;
            customer.Phonenumber = request.Phonenumber;
            customer.Address = request.Address;
            customer.Email = request.Email;
            customer.IsActive = request.IsActive;
            customer.User = user;
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(customer);
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerRequest request)
        {
            string email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(user == null)
            {
                return BadRequest("Usuario no existe");
            }

            Customer oldCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if(oldCustomer != null)
            {
                return BadRequest("Ya existe un cliente registrado con ese email.");
            }
            Customer customer = new Customer
            {
                FirtsName = request.FirtsName,
                LastName = request.LastName,
                Phonenumber = request.Phonenumber,
                Address = request.Address,
                Email = request.Email,
                IsActive = request.IsActive,
                User = user
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
