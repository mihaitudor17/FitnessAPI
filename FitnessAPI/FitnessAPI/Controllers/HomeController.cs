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
                foreach(var role in Enum.GetValues(typeof(RoleType)))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role.ToString()));
                }
                var test = _roleManager.Roles.ToList();
                var user = await _userManager.FindByNameAsync(userDto.UserName);
                await _userManager.AddToRoleAsync(user, user.Role.ToString());
                return Ok(new { token = _authorization.GetToken(user, user.Role.ToString()) });
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return BadRequest(ModelState);
            }
        }

        //[Authorize(Roles = "User")]
        [HttpPost("Add Person")]
        public async Task<ActionResult<Person>> AddPerson(PersonDto person)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            // Check if the current user already has a person entity associated with it
            var existingPerson = await _dbContext.Users.FirstOrDefaultAsync(u => u.PersonId != 0);
            if (existingPerson != null)
            {
                return Conflict("A person entity already exists for the current user.");
            }

            // Create a new person entity and set its UserId property to the current user's Id
            var newPerson = new Person
            {
                Name = person.Name,
                Age = person.Age,
                Height = person.Height,
                Weight = person.Weight,
                Gender = person.Gender,
            };

            // Add the new person entity to the People DbSet and save the changes
            _dbContext.People.Add(newPerson);
            await _dbContext.SaveChangesAsync();
            currentUser.PersonId = newPerson.Id;
            await _userManager.UpdateAsync(currentUser);

            // Return a 201 Created response with the new person entity as the response body
            return CreatedAtAction(nameof(AddPerson), new { id = newPerson.Id }, newPerson);
        }

        //[Authorize(Roles = "User")]
        [HttpPost("Add Workout")]
        public async Task<ActionResult<Workout>> AddWorkout(WorkoutDto workout)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            // Check if the current user already has a person entity associated with it
            var existingPerson = await _dbContext.Users.FirstOrDefaultAsync(u => u.PersonId != 0);
            if (existingPerson == null)
            {
                return Conflict("Please add your info first.");
            }
            var person = _dbContext.People.FirstOrDefault(p => p.Id == currentUser.PersonId);
            // Create a new person entity and set its UserId property to the current user's Id
            var newWorkout = new Workout
            {
                Date = DateOnly.FromDateTime(DateTime.Today),
                Duration= workout.Duration,
                Intensity= workout.Intensity,
                Exercise = workout.Exercise,
                PersonId = person.Id
            };

            // Add the new person entity to the People DbSet and save the changes
            person.Workouts.Add(newWorkout);
            await _dbContext.SaveChangesAsync();

            // Return a 201 Created response with the new person entity as the response body
            return CreatedAtAction(nameof(AddWorkout), new { id = newWorkout.Id }, newWorkout);
        }

        //[Authorize(Roles = "User")]
        [HttpGet("Total calories")]
        public async Task<IActionResult> GetPersonAndWorkout()
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Find the person associated with the current user
            var personWithWorkouts = await _dbContext.People
                                    .Include(p => p.Workouts)
                                    .Where(p => p.Id == currentUser.PersonId)
                                    .FirstOrDefaultAsync();

            if (personWithWorkouts == null)
            {
                return NotFound();
            }

            var calories = Calculator.GetTotalCalories(personWithWorkouts);

            // Return the person and their latest workout
            return Ok(new { Calories = calories });
        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete("Delete user records")]
        public async Task<IActionResult> DeletePersonAndWorkoutsForUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            // Get the person for the user
            var person = await _dbContext.People.FirstOrDefaultAsync(p => p.Id == user.PersonId );

            if (person == null)
            {
                return NotFound();
            }

            // Delete all workouts for the person
            var workouts = await _dbContext.Workouts.Where(w => w.PersonId == person.Id).ToListAsync();
            _dbContext.Workouts.RemoveRange(workouts);

            // Delete the person for the user
            _dbContext.People.Remove(person);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

    }
}