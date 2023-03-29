using Core.Dtos;
using FitnessApi.Dtos;
using FitnessApi.Entities;
using FitnessApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FintessApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private UserService userService { get; set; }


        public UsersController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("/get-all")]
        public ActionResult<List<User>> GetAll()
        {
            var results = userService.GetAll();

            return Ok(results);
        }

        [HttpGet("/get/{userId}")]
        public ActionResult<User> GetById(int userId)
        {
            var result = userService.GetById(userId);

            if (result == null)
            {
                return BadRequest("User not fount");
            }

            return Ok(result);
        }

        [HttpPatch("edit-name")]
        public ActionResult<bool> GetById([FromBody] UserUpdateDto userUpdateModel)
        {
            var result = userService.EditPassword(userUpdateModel);

            if (!result)
            {
                return BadRequest("User could not be updated.");
            }

            return result;
        }

        //[HttpPost("grades-by-course")]
        //public ActionResult<GradesByUser> Get_CourseGrades_ByUserId([FromBody] UserGradesRequest request)
        //{
        //    var result = userService.GetGradesById(request.UserId, request.CourseType);
        //    return Ok(result);
        //}

        [HttpDelete("delete/{userId}")]
        public ActionResult<UserDto> DeleteUser(int userId)
        {
            try
            {
                var userToDelete =userService.GetById(userId);

                if (userToDelete == null)
                {
                    return NotFound($"User not found");
                }

                return userService.DeleteUser(userId);
            }
            catch (Exception)
            {
                return BadRequest("User could not be deleted.");
            }

        }
    }
}
