using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using insurance_service.Controllers;
using insurance_service.Message;
using insurance_service.Model;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using Xunit;

namespace insurance_service.tests.Controllers
{
    public class InsuranceControllerTests
    {
        private readonly InsuranceController _controller;
        private readonly Mock<ILogger<InsuranceController>> _loggerMock;
        private readonly MockHttpMessageHandler _mockHttp;
        private readonly HttpClient _httpClient;

        public InsuranceControllerTests()
        {
            _loggerMock = new Mock<ILogger<InsuranceController>>();
            _mockHttp = new MockHttpMessageHandler();
            _httpClient = new HttpClient(_mockHttp);
            _controller = new InsuranceController(_httpClient, _loggerMock.Object);
        }

        [Fact]
        public void FindAll_ReturnsAllInsurances()
        {
            // Act
            var result = _controller.FindAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count); // Based on the default data in controller
            Assert.Contains(result, i => i.Type == InsuranceType.Life);
            Assert.Contains(result, i => i.Type == InsuranceType.Medical);
        }

        [Fact]
        public void FindById_ReturnsCorrectInsurance()
        {
            // Act
            var result = _controller.FindById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(3, result.PersonId);
            Assert.Equal(10000, result.Amount);
            Assert.Equal(InsuranceType.Life, result.Type);
        }

        [Fact]
        public void FindById_WithInvalidId_ReturnsNull()
        {
            // Act
            var result = _controller.FindById(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void AddNew_CreatesNewInsurance()
        {
            // Arrange
            var newInsurance = new Insurance
            {
                PersonId = 4,
                Amount = 25000,
                Expiry = DateTime.Today.AddYears(1),
                Type = InsuranceType.Medical
            };

            // Act
            var result = _controller.AddNew(newInsurance);

            // Assert
            Assert.NotEqual(0, result.Id);
            Assert.Equal(newInsurance.PersonId, result.PersonId);
            Assert.Equal(newInsurance.Amount, result.Amount);
            Assert.Equal(newInsurance.Type, result.Type);
        }

        [Fact]
        public void FindDetailsById_ReturnsInsuranceWithPersonDetails()
        {
            // Arrange
            var person = new Person { Id = 3, Name = "Test Person", Age = 30 };
            var personJson = JsonSerializer.Serialize(person);

            _mockHttp.When("http://person-service:8080/1")
                    .Respond("application/json", personJson);

            // Act
            var result = _controller.FindDetailsById(1);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Insurance);
            Assert.NotNull(result.Person);
            Assert.Equal(1, result.Insurance.Id);
            Assert.Equal(person.Id, result.Person.Id);
            Assert.Equal(person.Name, result.Person.Name);
            Assert.Equal(person.Age, result.Person.Age);
        }

        [Fact]
        public void FindDetailsById_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _mockHttp.When("http://person-service:8080/999")
                    .Respond(HttpStatusCode.NotFound);

            // Act & Assert
            Assert.Throws<HttpRequestException>(() => _controller.FindDetailsById(999));
        }
    }
}
