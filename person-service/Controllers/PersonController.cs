using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using web_api_example.Model;
using web_api_example.Data;

namespace web_api_example.Controllers
{
    [ApiController]
    [Route("persons/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly ILogger<PersonController> _logger;
        private readonly List<Person> _persons = new List<Person>();
        private readonly PersonsDbContext _context;

        public PersonController(ILogger<PersonController> logger, PersonsDbContext context)
        {
            _logger = logger;
            _context = context;
            _persons.Add(new Person {Id = 1, Name = "Test1", Age = 20});
            _persons.Add(new Person {Id = 2, Name = "Test2", Age = 30});
            _persons.Add(new Person {Id = 3, Name = "Test3", Age = 40});
        }

        [HttpGet]
        [Route("/")]
        public List<Person> FindAll()
        {
            _logger.LogInformation("Find All");
            // return _persons;
            // using var db = new PersonsDbContext();
            return _context.Persons.ToList();
        }
        
        [HttpGet]
        [Route("/{id:int}")]
        public Person FindById([FromRoute] int id)
        {
            _logger.LogInformation("Find By Id={Id}", id);
            // return _persons.Find(person => person.Id == id);
            // using var db = new PersonsDbContext();
            return _context.Persons.Find(id);
        }

        [HttpGet]
        [Route("/age-greater-than/{age:int}")]
        public List<Person> FindByAgeGreaterThan([FromRoute] int age) {
            _logger.LogInformation("Find By Age>{age}", age);
            // using var db = new PersonsDbContext();
            return _context.Persons.Where(person => person.Age > age).ToList();
            // return _persons.FindAll(person => person.Age > age);
        }

        [HttpPost]
        [Route("/")]
        public Person AddNew([FromBody] Person person)
        {
            _logger.LogInformation("Add New Name={Name}", person.Name);
            // using var db = new PersonsDbContext();
            var addedPerson = _context.Persons.Add(person);
            _context.SaveChanges();
            // person.Id = _persons.Count + 1;
            // _persons.Add(person);
            return person;
        }
    }
}