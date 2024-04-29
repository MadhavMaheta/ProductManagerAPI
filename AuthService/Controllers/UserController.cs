using AuthService.DTO;
using AuthService.Helper;
using AuthService.Models;
using AuthService.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IUserService _userService { get; set; }

        private readonly IMapper _mapper;

        public UserController(IUserService userService, IConfiguration config, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult RegisterUser(User objUser)
        {
            ModelState.Remove("Id");

            if (ModelState.IsValid)
            {
                if(_userService.GetUserByEmailId(objUser.Email) != null)
                    return BadRequest("User with same email already exist");

                if (_userService.RegisterUser(objUser))
                {
                    return Ok(objUser);
                }
                else
                {
                    return BadRequest("Error in registering user");
                }
            }
            else
            {
                return BadRequest("Invalid data");
            }
        }

        [HttpPatch]
        [Authorize("Admin", "Customer")]
        public IActionResult UpdateUser(User objUser)
        {
            if (ModelState.IsValid)
            {
                if (_userService.UpdateUser(objUser))
                {
                    return Ok(objUser);
                }
                else
                {
                    return BadRequest("Error in updating user");
                }
            }
            else
            {
                return BadRequest("Invalid data");
            }
        }

        [HttpPatch]
        [Authorize("Admin", "Customer")]
        public IActionResult ChangeUserPassword(UserSetPassword objUserSetPassword)
        {
            if (ModelState.IsValid)
            {
                if (_userService.ChangeUserPassword(objUserSetPassword))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Error in updating user password");
                }
            }
            else
            {
                return BadRequest("Invalid data");
            }
        }

        [HttpGet("{nUserId}")]
        [Authorize("Admin", "Customer")]
        public IActionResult GetUser(int nUserId)
        {
            User objUser = _userService.GetUser(nUserId);
            return Ok(_mapper.Map<UserDTO>(objUser));
        }

        [HttpGet]
        [Authorize("Admin", "Customer")]
        public List<UserDTO> GetUserList()
        {
            List<User> lstUsers = _userService.GetUserList();
            return _mapper.Map<List<UserDTO>>(lstUsers);
        }
    }
}
