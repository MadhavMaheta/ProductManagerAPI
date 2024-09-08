using AuthService.Controllers;
using AuthService.Models;
using AuthService.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Auth.Test
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockMapper = new Mock<IMapper>();
            _userController = new UserController(_mockUserService.Object, _mockMapper.Object);
        }

        [Fact]
        public void RegisterUser_ReturnOKActionResult_WhenUserIsValid()
        {
            // Arange
            var user = new User() { Id = 1, Email = "abcd@test.com", Name = "Abcd", Password = "Admin", Role = Role.Admin };
            _mockUserService.Setup(x => x.GetUserByEmailId(user.Email)).Returns((User)null);
            _mockUserService.Setup(x => x.RegisterUser(user)).Returns(true);

            // Act
            var result = _userController.RegisterUser(user) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(user, result.Value);
        }

        [Fact]
        public void RegisterUser_ReturnBadRequestActionResult_WhenUserEmailExists()
        {
            // Arange
            var user = new User() { Id = 1, Email = "abcd@test.com", Name = "Abcd", Password = "Admin", Role = Role.Admin };
            _mockUserService.Setup(x => x.GetUserByEmailId(user.Email)).Returns(user);

            // Act
            var result = _userController.RegisterUser(user) as BadRequestObjectResult;

            // Assert
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("User with same email already exist",result.Value);
        }

        [Fact]
        public void RegisterUser_ReturnBadRequestResult_WhenInvalidUserData()
        {
            // Arange
            var user = new User() { Id = 1, Email = null, Name = "Abcd", Password = "Admin", Role = Role.Customer };
            _userController.ModelState.AddModelError("Email", "Required");
            _mockUserService.Setup(x => x.RegisterUser(user)).Returns(false);

            // Act
            var result = _userController.RegisterUser(user) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Invalid data", result.Value);
        }

        [Fact]
        public void RegisterUser_ReturnBadRequestResult_WhenUserFailedToRegister()
        {
            // Arange
            var user = new User() { Id = 1, Email = "abcd@test.com", Name = "Abcd", Password = "Admin", Role = Role.Customer};
            _mockUserService.Setup(x => x.GetUserByEmailId(user.Email)).Returns((User)null);
            _mockUserService.Setup(x => x.RegisterUser(user)).Returns(false);

            // Act
            var result = _userController.RegisterUser(user) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Error in registering user", result.Value);
        }
    }
}
