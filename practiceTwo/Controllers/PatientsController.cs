using Microsoft.AspNetCore.Mvc;
using PatientManager.Managers;
using PatientManager.Models;
using System.Net.Http.Json;
using practiceTwo.Dtos;
using Serilog;

namespace practiceTwo.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientManager _manager;
        private readonly Serilog.ILogger _logger;
        private readonly HttpClient _http;

        public PatientsController(IPatientManager manager)
        {
            _manager = manager;
            _logger = Log.ForContext<PatientsController>();
            _http = new HttpClient();
        }

        // CREATE
        [HttpPost]
        public IActionResult Create([FromBody] PatientDto dto)
        {
            _logger.Information("Creating patient {CI}", dto.CI);
            var p = new Patient
            {
                Name = dto.Name,
                LastName = dto.LastName,
                CI = dto.CI
            };
            _manager.Create(p);
            return CreatedAtAction(nameof(GetByCi), new { ci = p.CI }, p);
        }

        // READ ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.Information("Getting all patients");
            return Ok(_manager.GetAll());
        }

        // READ ONE
        [HttpGet("{ci}")]
        public IActionResult GetByCi(string ci)
        {
            _logger.Information("Getting patient {CI}", ci);
            var p = _manager.GetByCi(ci);
            if (p == null)
            {
                _logger.Warning("Patient {CI} not found", ci);
                return NotFound("Patient not found");
            }
            return Ok(p);
        }

        // UPDATE
        [HttpPut("{ci}")]
        public IActionResult Update(string ci, [FromBody] PatientUpdateDto dto)
        {
            _logger.Information("Updating patient {CI}", ci);
            if (!_manager.Update(ci, dto.Name, dto.LastName))
            {
                _logger.Warning("Patient {CI} not found for update", ci);
                return NotFound("Patient not found");
            }
            return NoContent();
        }

        // DELETE
        [HttpDelete("{ci}")]
        public IActionResult Delete(string ci)
        {
            _logger.Information("Deleting patient {CI}", ci);
            if (!_manager.Delete(ci))
            {
                _logger.Warning("Patient {CI} not found for deletion", ci);
                return NotFound("Patient not found");
            }
            return NoContent();
        }

        [HttpGet("{ci}/gift")]
        public async Task<IActionResult> GetGift(string ci)
        {
            if (_manager.GetByCi(ci) == null)
                return NotFound("Patient not found");

            try
            {
                var gifts = await _http.GetFromJsonAsync<List<GiftDto>>(
                    "https://api.restful-api.dev/objects"
                 );
                        if (gifts == null || !gifts.Any())
                                return NoContent();

                var rndIndex = new Random().Next(gifts.Count);
                var chosenGift = gifts[rndIndex];
                        return Ok(chosenGift);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching gift for {CI}", ci);
                return StatusCode(500, "Failed to retrieve gift");
            }
        }
    }
}
