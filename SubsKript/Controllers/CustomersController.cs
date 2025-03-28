using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SubsKript.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        //example customer
        private static List<Customer> _customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Anastasya" },
            new Customer { Id = 2, Name = "Asya" },
            new Customer { Id = 3, Name = "Eda" },
            new Customer { Id = 3, Name = "Jack" },
        };

        // get api 
        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            return Ok(_customers);
        }

        // delete api 
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
                return NotFound();

            _customers.Remove(customer);
            return NoContent();
        }
    }

    // customer model 
    public class Customer
    {
        public int Id { get; set; }      
        public string Name { get; set; }
    }
}