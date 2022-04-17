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
    [Route("/poc/gsl/v1/user")]
    [ApiController]
    public class UserController : Controller
    {
        private UnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public UserController(UnitOfWork unitOfWork, ILoggerFactory logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger.CreateLogger("User");
        }

        private bool ValidatePermission([FromServices] OAuth oAuth, string userId, List<string> roles)
        {
            if (!oAuth.ValidatAccess(userId, roles)) return false;

            return true;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(List<User>), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public IActionResult Get([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator" })) return Unauthorized();

            var user = _unitOfWork
                .GetRepository<User>()
                .ToList();

            if (user.IsNull() || user.Count() <= 0)
            {
                return NotFound("Nenhuma ocorrencia encontrada.");
            }

            var userModels = user.Select(p => new User { Id = p.Id, Name = p.Name, Profile = p.Profile, Mail = p.Mail, Document = p.Document });

            return Ok(userModels);

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<User>), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> Get([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, string id)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator" })) return Unauthorized();

            var user = await _unitOfWork
                .GetRepository<User>()
                .FindAsync(id);

            if (user.IsNull())
            {
                return NotFound("Nenhuma ocorrencia encontrada.");
            }

            return Ok(user);
        }

        [HttpGet("Uid/{uId}")]
        [ProducesResponseType(typeof(List<User>), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> GetByUId([FromHeader(Name = "UserId")] string userId,[FromHeader(Name = "Authorization")][Required] string authorization, string uId)
        {

            var user = await _unitOfWork
                .GetRepository<User>()
                .FindByCustom("UId", uId);

            if (user.IsNull())
            {
                return NotFound("Nenhuma ocorrencia encontrada.");
            }

            return Ok(user);
        }

        [HttpGet("profile/{profile}")]
        [ProducesResponseType(typeof(List<User>), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> GetByProfile([FromHeader(Name = "UserId")] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, string profile)
        {

            var user = _unitOfWork
                .GetRepository<User>()
                .FindAllByCustom("Profile", profile);

            if (user.IsNull())
            {
                return NotFound("Nenhuma ocorrencia encontrada.");
            }

            return Ok(user);
        }


        [HttpPost()]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public IActionResult Post([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, [FromBody] User userModel)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator" })) return Unauthorized();

            var repo = _unitOfWork.GetRepository<User>();
            repo.Add(userModel);

            return Created("[controller]", userModel);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> Put([FromHeader(Name = "UserId")][Required] string userId, [FromHeader(Name = "Authorization")][Required] string authorization, [FromServices] OAuth oAuth, [FromBody] User userModel, string id)
        {
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator" })) return Unauthorized();

            var repo = _unitOfWork.GetRepository<User>();

            var existing = await repo.FindAsync(id);

            if (existing.IsNotNull())
            {
                userModel.Id = id;
                repo.Update(userModel);
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
            if (!ValidatePermission(oAuth, userId, new List<string> { "administrator", "collaborator" })) return Unauthorized();

            _unitOfWork.GetRepository<User>().Remove(id);

            return NoContent();
        }
    }
}
