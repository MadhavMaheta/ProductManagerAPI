using AuthService.Controllers;
using Xunit;
using FakeItEasy;
using Microsoft.Extensions.Configuration;
using AuthService.Services;
using Moq;
using System.Web.Http.Results;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Test
{
    public class UnitTest1
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly AuthController _controller;

        public UnitTest1()
        {
            _mockUserService = new Mock<IUserService>();
            _mockConfig = new Mock<IConfiguration>();
            _controller = new AuthController(_mockUserService.Object, _mockConfig.Object);
        }


        [Fact]
        public void Auth_Login_ReturnsUnauthorized_WhenUserNotFound()
        {
            // Arrange
            var loginModel = new LoginModel { Username = "test", Password = "test" };
            _mockUserService.Setup(service => service.GetUser(It.IsAny<string>(), It.IsAny<string>())).Returns((User)null);

            // Act
            var result = _controller.Login(loginModel);

            Assert.IsType<Microsoft.AspNetCore.Mvc.UnauthorizedResult>(result);
        }
    }
}