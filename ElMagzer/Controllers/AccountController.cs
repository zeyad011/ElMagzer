using ElMagzer.Core.Models.Identity;
using ElMagzer.Core.Services;
using ElMagzer.Shared.Dtos;
using ElMagzer.Shared.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ElMagzer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthService _authService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Name);
            
            if (user is null) return Unauthorized(new ApiResponse(401,"The Email is not Existed"));

            var result = await _signInManager.CheckPasswordSignInAsync(user,model.Password,false);
            
            if (!result.Succeeded) return Unauthorized(new ApiResponse(401,"Wrong Password"));

            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email ?? "N/A",
                Token = await _authService.CreateTokenAsync(user, _userManager),
            });
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(ReqisterDdto model)
        {
            var user = new AppUser()
            {
                DisplayName = model.UserName,
                Email = model.Email,
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
            };
            var result  = await _userManager.CreateAsync(user,model.Password);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager),
            });
            
        }
    }
}
 