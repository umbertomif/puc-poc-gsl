using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POC.GSL.Data;
using POC.GSL.Domain;

namespace POC.GSL.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("/poc/gsl/v1/healthz")]
    public class HealthzController : Controller
    {
        private UnitOfWork _unitOfWork;

        public HealthzController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// health check 
        /// </summary>
        /// <response code="200">healthz up</response>
        /// <response code="500">healthz down</response>
        [HttpGet]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(500)]
        public IActionResult Get()
        {

            var user = _unitOfWork.GetRepository<User>().ToList();

            return Ok();
        }
    }
}
