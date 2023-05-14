using FitnessAPI.DTOs;
using FitnessAPI.Entities;
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


        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager, CustomDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
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
                return Ok();
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
                Gender= person.Gender
            };

            // Add the new person entity to the People DbSet and save the changes
            _dbContext.People.Add(newPerson);
            currentUser.PersonId = newPerson.Id;
            await _userManager.UpdateAsync(currentUser);
            await _dbContext.SaveChangesAsync();

            // Return a 201 Created response with the new person entity as the response body
            return CreatedAtAction(nameof(AddPerson), new { id = newPerson.Id }, newPerson);
        }

        //[Authorize(Roles = "User")]
        [HttpPost("Add Workout")]
        public async Task<ActionResult<Workout>> AddWorkout(Workout workout)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            // Check if the current user already has a person entity associated with it
            var existingPerson = await _dbContext.Users.FirstOrDefaultAsync(u => u.PersonId != 0);
            if (existingPerson == null)
            {
                return Conflict("Please add your info first.");
            }

            // Create a new person entity and set its UserId property to the current user's Id
            var newWorkout = new Workout
            {
                Date = workout.Date,
                Duration= workout.Duration,
                Intensity= workout.Intensity,
                Exercise = workout.Exercise
            };

            // Add the new person entity to the People DbSet and save the changes
            _dbContext.Workouts.Add(newWorkout);
            _dbContext.People.Find(currentUser.PersonId).Workouts.Add(newWorkout);
            await _userManager.UpdateAsync(currentUser);
            await _dbContext.SaveChangesAsync();

            // Return a 201 Created response with the new person entity as the response body
            return CreatedAtAction(nameof(AddWorkout), new { id = newWorkout.Id }, newWorkout);
        }

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
    }
}