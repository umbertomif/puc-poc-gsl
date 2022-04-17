using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using POC.GSL.Data;
using POC.GSL.Domain;
using POC.GSL.Infrastructure;
using POC.GSL.WebApi.Services;

namespace POC.GSL.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("/poc/gsl/v1/merchandise")]
    [ApiController]
    public class MerchandiseController : Controller
    {
        private UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public MerchandiseController(UnitOfWork unitOfWork, ILoggerFactory logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger.CreateLogger("Merchandise");
        }

        private bool ValidatePermission([FromServices] OAuth oAuth, string userId, List<string> roles)
        {
            if (!oAuth.ValidatAccess(userId, roles)) return false;

            return true;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(List<Merchandise>), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public IActionResult Get([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "supplier", "customer" })) return Unauthorized();

            var merchandise = _unitOfWork
                .GetRepository<Merchandise>()
                .ToList();

            if (merchandise.IsNull() || merchandise.Count() <= 0)
            {
                return NotFound("Nenhuma ocorrencia encontrada.");
            }

            var merchandiseModels = merchandise.Select(p => new Merchandise { Id = p.Id, Name = p.Name, Description = p.Description, UserId = p.UserId, UnitaryValue = p.UnitaryValue });

            return Ok(merchandiseModels);

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<Merchandise>), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> Get([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, string id)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "supplier", "customer" })) return Unauthorized();

            var merchandise = await _unitOfWork
                .GetRepository<Merchandise>()
                .FindAsync(id);

            if (merchandise.IsNull())
            {
                return NotFound("Nenhuma ocorrencia encontrada.");
            }

            return Ok(merchandise);
        }


        [HttpPost()]
        [ProducesResponseType(typeof(Merchandise), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public IActionResult Post([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, [FromBody] Merchandise merchandiseModel)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "supplier", "customer" })) return Unauthorized();

            var repo = _unitOfWork.GetRepository<Merchandise>();
            repo.Add(merchandiseModel);

            return Created("[controller]", merchandiseModel);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Merchandise), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> Put([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, [FromBody] Merchandise merchandiseModel, string id)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "supplier", "customer" })) return Unauthorized();

            var repo = _unitOfWork.GetRepository<Merchandise>();

            var existing = await repo.FindAsync(id);

            if (existing.IsNotNull())
            {
                merchandiseModel.Id = id;
                repo.Update(merchandiseModel);
            }
            else
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, string id)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "supplier", "customer" })) return Unauthorized();

            _unitOfWork.GetRepository<Merchandise>().Remove(id);

            return NoContent();
        }
    }
}
