using AuthService.Controllers;
using AuthService.Models;
using AuthService.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Auth.Test
{
    public class AuthControllerTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthController _controller;

        public AuthControllerTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockConfig = new Mock<IConfiguration>();
            _controller = new AuthController(_mockUserService.Object, _mockConfig.Object);
        }

        [Fact]
        public void Login_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "test", Password = "test" };
            _mockUserService.Setup(service => service.GetUser(It.IsAny<string>(), It.IsAny<string>())).Returns((User)null);

            // Act
            var result = _controller.Login(loginModel);

            //Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.UnauthorizedResult>(result);
        }

        [Fact(Skip = "Logic not implemented")]
        public void Login_ReturnsOk_WhenUserFound()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "test", Password = "test" };
            _mockUserService.Setup(service => service.GetUser(It.IsAny<string>(), It.IsAny<string>())).Returns((User)null);

            // Act
            var result = _controller.Login(loginModel);

            //Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.UnauthorizedResult>(result);
        }
    }
}