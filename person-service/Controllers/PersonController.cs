using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using web_api_example.Model;

namespace web_api_example.Controllers
{
    [ApiController]
    [Route("persons/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly ILogger<PersonController> _logger;
        private readonly List<Person> _persons = new List<Person>();

        public PersonController(ILogger<PersonController> logger)
        {
            _logger = logger;
            _persons.Add(new Person {Id = 1, Name = "Test1", Age = 20});
            _persons.Add(new Person {Id = 2, Name = "Test2", Age = 30});
            _persons.Add(new Person {Id = 3, Name = "Test3", Age = 40});
        }

        [HttpGet]
        [Route("/")]
        public List<Person> FindAll()
        {
            _logger.LogInformation("Find All");
            return _persons;
        }
        
        [HttpGet]
        [Route("/{id:int}")]
        public Person FindById([FromRoute] int id)
        {
            _logger.LogInformation("Find By Id={Id}", id);
            return _persons.Find(person => person.Id == id);
        }

        [HttpPost]
        [Route("/")]
        public Person AddNew([FromBody] Person person)
        {
            _logger.LogInformation("Add New name={Name}", person.Name);
            person.Id = _persons.Count + 1;
            _persons.Add(person);
            return person;
        }
    }
}