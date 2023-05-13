using FitnessAPI.DTOs;
using FitnessAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;


        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> CreateUser(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User { UserName = userDto.UserName };
            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            var result = await _signInManager.PasswordSignInAsync(userDto.UserName, userDto.Password, false, false);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return BadRequest(ModelState);
            }
        }
    }
}