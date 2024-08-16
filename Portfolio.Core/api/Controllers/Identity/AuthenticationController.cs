using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Core.Exceptions;
using Portfolio.Core.Interfaces.Identity;
using Portfolio.Core.Types.DTOs.Requests;
using Portfolio.Core.Types.DTOs.Requests.Identity;
using Portfolio.Core.Types.DTOs.Responses;
using Portfolio.Core.Types.DTOs.Users;

namespace Portfolio.Core.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<BaseResponse> RegisterAsync([FromBody] IdentityBaseRequest model)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _authenticationService.RegisterAsync(new UserDTO { Email = model?.Email, Password = model?.Password, ConfirmPassword = model?.ConfirmPassword }) ?
                    new BaseResponse { Success = true } :
                    throw new Exception("Error registering user. Please notify me to resolve this."); 
                });            
        }

        [HttpPost("registerAdmin")]
        [Authorize(Policy = "Admin")]
        public async Task<BaseResponse> RegisterAdminAsync([FromBody] IdentityBaseRequest model)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _authenticationService.RegisterAsync(new UserDTO { Email = model?.Email, Password = model?.Password, ConfirmPassword = model?.ConfirmPassword }, true) ?
                    new BaseResponse { Success = true } :
                    throw new Exception("Error registering user. Please notify me to resolve this.");
                });            
        }

        [HttpPost("login")]
        public async Task<BaseResponse> LoginAsync([FromBody] IdentityBaseRequest model)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    string token = await _authenticationService.LoginAsync(new UserDTO { Email = model?.Email, Password = model?.Password, ConfirmPassword = model?.ConfirmPassword });
                    return !string.IsNullOrEmpty(token) ? new BaseResponse { Success = true } : throw new Exception("Invalid username or password.");
                });  
        }

        [HttpPost("logout")]
        public async Task<BaseResponse> LogoutAsync([FromBody] BaseRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    bool success = await _authenticationService?.LogoutAsync(request?.Email);
                    return success ? new BaseResponse { Success = true } : throw new Exception("An error occurred during Logout process.");
                });            
        }

        [HttpPost("forgotPassword")]
        public async Task<BaseResponse> ForgotPasswordAsync([FromBody] IdentityBaseRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _authenticationService.ForgotPasswordAsync(request) ? new BaseResponse { Success = true } : throw new Exception("An error occurred during password reset.");
                });              
        }

        [HttpPost("forgotPasswordAdmin")]
        [Authorize(Policy = "Admin")]
        public async Task<BaseResponse> ForgotPasswordAdminAsync([FromBody] IdentityBaseRequest request)
        {
            return await DefaultException.ExceptionControllerHandler(async () => { 
                    return await _authenticationService.ForgotPasswordAsync(request, true) ? new BaseResponse { Success = true } : throw new Exception("An error occurred during password reset.");
                });              
        }
    }
}