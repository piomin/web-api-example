using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using web_api_example.Controllers;
using web_api_example.Data;
using web_api_example.Model;
using Xunit;

namespace web_api_example.tests.Controllers
{
    public class PersonControllerTests
    {
        private readonly PersonsDbContext _context;
        private readonly PersonController _controller;
        private readonly Mock<ILogger<PersonController>> _loggerMock;

        public PersonControllerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<PersonsDbContext>()
                .UseInMemoryDatabase(databaseName: "TestPersonDb")
                .Options;

            _context = new PersonsDbContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // Setup logger mock
            _loggerMock = new Mock<ILogger<PersonController>>();

            // Create controller instance
            _controller = new PersonController(_loggerMock.Object, _context);
        }

        [Fact]
        public void FindAll_ReturnsAllPersons()
        {
            // Arrange
            var testPerson = new Person { Name = "Test Person", Age = 25 };
            _context.Persons.Add(testPerson);
            _context.SaveChanges();

            // Act
            var result = _controller.FindAll();

            // Assert
            Assert.Single(result);
            Assert.Equal(testPerson.Name, result.First().Name);
            Assert.Equal(testPerson.Age, result.First().Age);
        }

        [Fact]
        public void FindById_ReturnsCorrectPerson()
        {
            // Arrange
            var testPerson = new Person { Name = "Test Person", Age = 25 };
            _context.Persons.Add(testPerson);
            _context.SaveChanges();

            // Act
            var result = _controller.FindById(testPerson.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testPerson.Name, result.Name);
            Assert.Equal(testPerson.Age, result.Age);
        }

        [Fact]
        public void FindByAgeGreaterThan_ReturnsCorrectPersons()
        {
            // Arrange
            var persons = new[]
            {
                new Person { Name = "Young", Age = 20 },
                new Person { Name = "Middle", Age = 30 },
                new Person { Name = "Old", Age = 40 }
            };
            _context.Persons.AddRange(persons);
            _context.SaveChanges();

            // Act
            var result = _controller.FindByAgeGreaterThan(25);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.True(p.Age > 25));
        }

        [Fact]
        public void AddNew_CreatesNewPerson()
        {
            // Arrange
            var newPerson = new Person { Name = "New Person", Age = 25 };

            // Act
            var result = _controller.AddNew(newPerson);

            // Assert
            Assert.NotEqual(0, result.Id);
            var savedPerson = _context.Persons.Find(result.Id);
            Assert.NotNull(savedPerson);
            Assert.Equal(newPerson.Name, savedPerson.Name);
            Assert.Equal(newPerson.Age, savedPerson.Age);
        }
    }
}
