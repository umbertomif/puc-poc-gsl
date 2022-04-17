using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using POC.GSL.Data;
using POC.GSL.Domain;
using POC.GSL.Infrastructure;
using POC.GSL.WebApi.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using POC.GSL.WebApi.Model;

namespace POC.GSL.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("/poc/gsl/v1/deposit")]
    [ApiController]
    public class DepositController : Controller
    {
        private UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public DepositController(UnitOfWork unitOfWork, ILoggerFactory logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger.CreateLogger("Deposit");
        }

        private bool ValidatePermission([FromServices] OAuth oAuth, string userId, List<string> roles)
        {
            if (!oAuth.ValidatAccess(userId, roles)) return false;

            return true;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(List<Deposit>), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public IActionResult Get([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth)
        {

            _logger.LogInformation(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "START"), userId);

            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "customer" }))
            {
                _logger.LogError(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "UNAUTHORIZED"), userId);
                return Unauthorized();
            }

            //emailService.SendEmailAsync("umberto.mif@gmail.com", "Teste de romulo", "teste de msg").GetAwaiter();

            var deposit = _unitOfWork
                .GetRepository<Deposit>()

                .ToList();

            if (deposit.IsNull() || deposit.Count() <= 0)
            {
                _logger.LogInformation(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "NOTFOUND"), userId);
                return NotFound("Nenhuma ocorrencia encontrada.");
            }

            var depositModels = deposit.Select(p => new Deposit { Id = p.Id, Name = p.Name, Address = p.Address, Phone = p.Phone, Mail = p.Mail });

            _logger.LogInformation(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2} - STATUS_CODE: {3} - RESULT: {4}", userId, DateTime.Now, "END", Ok().StatusCode, userId), userId);
            return Ok(depositModels);

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<Deposit>), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> Get([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, string id)
        {

            _logger.LogInformation(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "START"), userId);

            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "customer" }))
            {
                _logger.LogError(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "UNAUTHORIZED"), userId);
                return Unauthorized();
            }

            var deposit = await _unitOfWork
                .GetRepository<Deposit>()
                .FindAsync(id);

            if (deposit.IsNull())
            {
                _logger.LogInformation(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "NOTFOUND"), userId);
                return NotFound("Nenhuma ocorrencia encontrada.");
            }

            _logger.LogInformation(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2} - STATUS_CODE: {3} - RESULT: {4}", userId, DateTime.Now, "END", Ok().StatusCode, JsonSerializer.Serialize(deposit)), JsonSerializer.Serialize(deposit));

            return Ok(deposit);
        }

        [HttpPost()]
        [ProducesResponseType(typeof(Deposit), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public IActionResult Post([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, [FromBody] Deposit depositModel)
        {
            _logger.LogInformation(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "START"), userId);

            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "customer" }))
            {
                _logger.LogError(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "UNAUTHORIZED"), userId);
                return Unauthorized();
            }

            var repo = _unitOfWork.GetRepository<Deposit>();
            repo.Add(depositModel);

            _logger.LogInformation(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2} - STATUS_CODE: {3} - RESULT: {4}", userId, DateTime.Now, "END", Ok().StatusCode, JsonSerializer.Serialize(depositModel)), JsonSerializer.Serialize(depositModel));

            return Created("[controller]", depositModel);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Deposit), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> Put([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, [FromBody] Deposit depositModel, string id)
        {
            _logger.LogInformation(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "START"), userId);

            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "customer" }))
            {
                _logger.LogError(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "UNAUTHORIZED"), userId);
                return Unauthorized();
            }

            var repo = _unitOfWork.GetRepository<Deposit>();

            var existing = await repo.FindAsync(id);

            if (existing.IsNotNull())
            {
                depositModel.Id = id;
                repo.Update(depositModel);
            }
            else
            {
                _logger.LogInformation(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "NOTFOUND"), userId);
                return NotFound();
            }
                
            return Ok();

        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, string id)
        {
            _logger.LogInformation(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "START"), userId);

            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "customer" }))
            {
                _logger.LogError(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2}", userId, DateTime.Now, "UNAUTHORIZED"), userId);
                return Unauthorized();
            }

            _unitOfWork.GetRepository<Deposit>().Remove(id);

            _logger.LogInformation(LogEvents.DepositList, String.Format("USER: {0} - DATE: {1} - SITUATION: {2} - STATUS_CODE: {3} - RESULT: {4}", userId, DateTime.Now, "END", Ok().StatusCode, id), id);

            return NoContent();
        }
    }
}