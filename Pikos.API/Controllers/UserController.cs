using Microsoft.AspNetCore.Mvc;
using Pikos.DAL;
using Pikos.DAL.Contracts;

namespace Pikos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserRepository userRepository;

        public UserController(IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            this.unitOfWork = unitOfWork;
            this.userRepository = userRepository;
        }

        [Route("user")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
