using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using insurance_service.Message;
using insurance_service.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace insurance_service.Controllers
{
    [ApiController]
    [Route("insurances/[controller]")]
    public class InsuranceController : ControllerBase
    {
        private readonly ILogger<InsuranceController> _logger;
        private readonly HttpClient _client;
        private readonly List<Insurance> _insurances = new List<Insurance>();

        public InsuranceController(HttpClient client, ILogger<InsuranceController> logger)
        {
            _client = client;
            _logger = logger;
            _insurances.Add(new Insurance { Id = 1, PersonId = 3, Amount = 10000, Expiry = DateTime.Today, Type = InsuranceType.Life});
            _insurances.Add(new Insurance { Id = 2, PersonId = 3, Amount = 5000, Expiry = DateTime.Today, Type = InsuranceType.Medical});
            _insurances.Add(new Insurance { Id = 3, PersonId = 2, Amount = 20000, Expiry = DateTime.Today, Type = InsuranceType.Pension});
            _insurances.Add(new Insurance { Id = 4, PersonId = 1, Amount = 30000, Expiry = DateTime.Today, Type = InsuranceType.Accident});
            _insurances.Add(new Insurance { Id = 5, PersonId = 1, Amount = 15000, Expiry = DateTime.Today, Type = InsuranceType.Life});
        }

        [HttpGet]
        [Route("/")]
        public List<Insurance> FindAll()
        {
            _logger.LogInformation("Find All");
            return _insurances;
        }

        [HttpGet]
        [Route("/{id:int}")]
        public Insurance FindById([FromRoute] int id)
        {
            _logger.LogInformation("Find By Id={Id}", id);
            return _insurances.Find(insurance => insurance.Id == id);
        }
        
        [HttpPost]
        [Route("/")]
        public Insurance AddNew([FromBody] Insurance insurance)
        {
            _logger.LogInformation("Add New PersonId={PersonId}", insurance.PersonId);
            insurance.Id = _insurances.Count + 1;
            _insurances.Add(insurance);
            return insurance;
        }

        [HttpGet]
        [Route("/{id:int}/details")]
        public async Task<InsuranceDetails> FindDetailsById([FromRoute] int id)
        {
            _logger.LogInformation("Find Details By Id={Id}", id);
            var insurance = _insurances.Find(insurance => insurance.Id == id);
            var webRequest = new HttpRequestMessage(HttpMethod.Get, "http://person-service:8080/" + id);
            var responseMessage = await _client.SendAsync(webRequest);
            responseMessage.EnsureSuccessStatusCode();
            var content = await responseMessage.Content.ReadAsStringAsync();
            _logger.LogInformation("Client: {msg}", content);
            var person = JsonSerializer.Deserialize<Person>(content);
            return new InsuranceDetails(insurance, person);
        }
    }
}