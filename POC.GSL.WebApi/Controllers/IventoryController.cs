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
    [Route("/poc/gsl/v1/iventory")]
    [ApiController]
    public class IventoryController : Controller
    {
        private UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public IventoryController(UnitOfWork unitOfWork, ILoggerFactory logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger.CreateLogger("Iventory");
        }

        private bool ValidatePermission([FromServices] OAuth oAuth, string userId, List<string> roles)
        {
            if (!oAuth.ValidatAccess(userId, roles)) return false;

            return true;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(List<Iventory>), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public IActionResult Get([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "customer" })) return Unauthorized();

            var model = _unitOfWork
                .GetRepository<Iventory>()
                .ToList();

            if (model.IsNull() || model.Count() <= 0)
            {
                return NotFound("Nenhuma ocorrencia encontrada.");
            }

            var depositModels = model.Select(p => new Iventory { Id = p.Id, MerchandiseId = p.MerchandiseId, DepositId = p.DepositId, InventoryQuantity = p.InventoryQuantity });

            return Ok(depositModels);

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<Iventory>), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> Get([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, string id)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "customer" })) return Unauthorized();

            var model = await _unitOfWork
                .GetRepository<Iventory>()
                .FindAsync(id);

            if (model.IsNull())
            {
                return NotFound("Nenhuma ocorrencia encontrada.");
            }

            return Ok(model);
        }


        [HttpPost()]
        [ProducesResponseType(typeof(Iventory), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public IActionResult Post([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, [FromBody] Iventory model)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "customer" })) return Unauthorized();

            var repo = _unitOfWork.GetRepository<Iventory>();
            repo.Add(model);

            return Created("[controller]", model);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Iventory), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> Put([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, [FromBody] Iventory model, string id)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator" })) return Unauthorized();

            var repo = _unitOfWork.GetRepository<Iventory>();

            var existing = await repo.FindAsync(id);

            if (existing.IsNotNull())
            {
                model.Id = id;
                repo.Update(model);
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
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator", "customer" })) return Unauthorized();

            _unitOfWork.GetRepository<Iventory>().Remove(id);

            return NoContent();
        }
    }
}
