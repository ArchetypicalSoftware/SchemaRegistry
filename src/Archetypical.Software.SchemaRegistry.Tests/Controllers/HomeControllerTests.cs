using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Archetypical.Software.SchemaRegistry.Controllers;
using Archetypical.Software.SchemaRegistry.Models;
using Xunit;

namespace Archetypical.Software.SchemaRegistry.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void HomeControllerIndex()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object);

            //Act
            var result = controller.Index();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void HomeControllerPrivacy()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object);

            //Act
            var result = controller.Privacy();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void HomeControllerError()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            //Act
            var result = controller.Error();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            Assert.NotNull((result as ViewResult).Model);
            Assert.IsType<ErrorViewModel>((result as ViewResult).Model);
        }
    }
}