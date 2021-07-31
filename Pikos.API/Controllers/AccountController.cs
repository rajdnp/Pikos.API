using Microsoft.AspNetCore.Mvc;
using static Pikos.Models.Constants.PikosConstants;
using static Pikos.Helpers.ApiResponse;
using static Pikos.Models.DTOs.SignInDtos;
using static Pikos.Models.DTOs.SignUpDtos;
using Pikos.BLL.Interfaces;
using System.Threading.Tasks;

namespace Pikos.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [Route("signin")]
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInRequest model)
        {
            var result = await accountService.SignIn(model, GetIpAddress());
            if(result != null)
            {
                return Ok(new SuccessResponse
                { 
                    Status = ResponseStatus.SUCCESS,
                    Data = result
                });
            }

            return BadRequest(new ErrorResponse { Status = ResponseStatus.FAILED, Message = "Invalid credentials" });
        }

        [Route("signup")]
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpRequest model)
        {
            var result = await accountService.SignUp(model);
            if(result)
            {
                return Ok(new SuccessResponse { Status = ResponseStatus.SUCCESS, Data = model });
            }

            return BadRequest();
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
