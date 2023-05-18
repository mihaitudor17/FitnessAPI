using FitnessAPI.DTOs;
using FitnessAPI.Entities;
using FitnessAPI.Enums;
using FitnessAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace FitnessAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly CustomDbContext _dbContext;
        private readonly AuthorizationService _authorization;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager, CustomDbContext dbContext, AuthorizationService authorization, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _authorization = authorization;
            _roleManager = roleManager;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> CreateUser(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User { UserName = userDto.UserName, Role = userDto.Role };
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
                var user = await _userManager.FindByNameAsync(userDto.UserName);
                await _userManager.AddToRoleAsync(user, user.Role.ToString());
                Response.Cookies.Append("jwtToken", _authorization.GetToken(user, user.Role.ToString()), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
                return Ok(new { message = $"Logged in with the role {user.Role.ToString()}" });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return BadRequest(ModelState);
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost("Add Person")]
        public async Task<ActionResult<Person>> AddPerson(PersonDto person)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var existingPerson = await _dbContext.Users.FirstOrDefaultAsync(u => u.PersonId != 0);
            if (existingPerson != null)
            {
                return Conflict("A person entity already exists for the current user.");
            }
            var newPerson = new Person
            {
                Name = person.Name,
                Age = person.Age,
                Height = person.Height,
                Weight = person.Weight,
                Gender = person.Gender,
            };
            _dbContext.People.Add(newPerson);
            await _dbContext.SaveChangesAsync();
            currentUser.PersonId = newPerson.Id;
            await _userManager.UpdateAsync(currentUser);
            return CreatedAtAction(nameof(AddPerson), new { id = newPerson.Id }, newPerson);
        }

        [Authorize(Roles = "User")]
        [HttpPost("Add Workout")]
        public async Task<ActionResult<Workout>> AddWorkout(WorkoutDto workout)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var existingPerson = await _dbContext.Users.FirstOrDefaultAsync(u => u.PersonId != 0);
            if (existingPerson == null)
            {
                return Conflict("Please add your info first.");
            }
            var person = _dbContext.People.FirstOrDefault(p => p.Id == currentUser.PersonId);
            var newWorkout = new Workout
            {
                Date = DateOnly.FromDateTime(DateTime.Today),
                Duration= workout.Duration,
                Intensity= workout.Intensity,
                Exercise = workout.Exercise,
                PersonId = person.Id
            };
            person.Workouts.Add(newWorkout);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(AddWorkout), new { id = newWorkout.Id }, newWorkout);
        }

        [Authorize(Roles = "User")]
        [HttpGet("Total calories")]
        public async Task<IActionResult> GetPersonAndWorkout()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var personWithWorkouts = await _dbContext.People
                                    .Include(p => p.Workouts)
                                    .Where(p => p.Id == currentUser.PersonId)
                                    .FirstOrDefaultAsync();
            if (personWithWorkouts == null)
            {
                return NotFound();
            }
            var calories = Calculator.GetTotalCalories(personWithWorkouts);
            return Ok(new { Calories = calories });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete user records")]
        public async Task<IActionResult> DeletePersonAndWorkoutsForUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound();
            }
            var person = await _dbContext.People.FirstOrDefaultAsync(p => p.Id == user.PersonId );
            if (person == null)
            {
                return NotFound();
            }
            var workouts = await _dbContext.Workouts.Where(w => w.PersonId == person.Id).ToListAsync();
            _dbContext.Workouts.RemoveRange(workouts);
            _dbContext.People.Remove(person);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "User")]
        [HttpGet("Get person")]
        public async Task<ActionResult<IEnumerable<Person>>> GetPeople()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var people = await _dbContext.People.Where(p => p.Id == currentUser.PersonId).ToListAsync();
            return Ok(people);
        }

        [Authorize(Roles = "User")]
        [HttpPut("Update person")]
        public async Task<IActionResult> UpdatePerson(PersonDto personDto)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var person = await _dbContext.People.FirstOrDefaultAsync(p => p.Id == currentUser.PersonId);
            if (person == null)
            {
                return NotFound();
            }

            person.Name = personDto.Name;
            person.Age = personDto.Age;
            person.Height = personDto.Height;
            person.Weight = personDto.Weight;
            person.Gender = personDto.Gender;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("Delete person")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var person = await _dbContext.People.FirstOrDefaultAsync(p => p.Id == currentUser.PersonId);
            if (person == null)
            {
                return NotFound();
            }

            var workouts = await _dbContext.Workouts.Where(w => w.PersonId == person.Id).ToListAsync();
            _dbContext.Workouts.RemoveRange(workouts);
            _dbContext.People.Remove(person);
            currentUser.PersonId = 0;
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Get users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")] 
        [HttpPut("Update user")]
        public async Task<IActionResult> UpdateUser(UserDto userDto)
        {
            var user = await _userManager.FindByNameAsync(userDto.UserName);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = userDto.UserName;
            if (!string.IsNullOrEmpty(userDto.Password))
            {
                var passwordValidator = HttpContext.RequestServices.GetRequiredService<IPasswordValidator<User>>();
                var passwordHasher = HttpContext.RequestServices.GetRequiredService<IPasswordHasher<User>>();

                var result = await passwordValidator.ValidateAsync(_userManager, user, userDto.Password);
                if (result.Succeeded)
                {
                    user.PasswordHash = passwordHasher.HashPassword(user, userDto.Password);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }

            return NoContent();
        }

        [Authorize(Roles = "User")]
        [HttpGet("Get workouts")]
        public async Task<ActionResult<IEnumerable<Workout>>> GetWorkouts()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var person = await _dbContext.People.FirstOrDefaultAsync(p => p.Id == user.PersonId);
            if (person == null)
            {
                return NotFound();
            }
            var workouts = await _dbContext.Workouts
                .Where(w => w.PersonId == person.Id)
                .ToListAsync();

            return Ok(workouts);
        }

        [Authorize(Roles = "User")]
        [HttpDelete("Delete workout")]
        public async Task<IActionResult> DeleteWorkout(int id)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var person = await _dbContext.People.FirstOrDefaultAsync(p => p.Id == currentUser.PersonId);
            if (person == null)
            {
                return NotFound();
            }
            var workout = await _dbContext.Workouts
                .FirstOrDefaultAsync(w => w.Id == id && w.PersonId == person.Id);

            if (workout == null)
            {
                return NotFound();
            }

            _dbContext.Workouts.Remove(workout);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "User")]
        [HttpPut("Update workout")]
        public async Task<IActionResult> UpdateWorkout(int id, WorkoutDto workoutDto)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var person = await _dbContext.People.FirstOrDefaultAsync(p => p.Id == currentUser.PersonId);
            if (person == null)
            {
                return NotFound();
            }
            var workout = await _dbContext.Workouts
                 .FirstOrDefaultAsync(w => w.Id == id && w.PersonId == person.Id);
            if (workout == null)
            {
                return NotFound();
            }
            workout.Intensity = workoutDto.Intensity;
            workout.Exercise = workoutDto.Exercise;
            workout.Duration = workoutDto.Duration;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

    }
}