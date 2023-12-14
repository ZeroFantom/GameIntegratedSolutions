using IntelliTrackSolutionsWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace IntelliTrackSolutionsWebTest.Controllers;

[TestFixture]
public class HomeControllerTests
{
    private HomeController _controller;
    private Mock<ILogger<HomeController>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(_loggerMock.Object);
    }

    [Test]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
    }

    [Test]
    public void Privacy_ReturnsViewResult()
    {
        // Act
        var result = _controller.Privacy();

        // Assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
    }

    [TestCase("200")]
    [TestCase("404")]
    [TestCase("500")]
    public void ErrorCodePage_ValidErrorCode_ReturnsViewResultWithModel(string errorCode)
    {
        // Act
        var result = _controller.ErrorCodePage(errorCode);

        // Assert
        Assert.That(result, Is.InstanceOf<ViewResult>());

        var viewResult = (ViewResult)result;
        Assert.That(viewResult.Model, Is.InstanceOf<string>());
    }

    [TestCase("abc")]
    [TestCase("")]
    [TestCase("1000")]
    public void ErrorCodePage_InvalidErrorCode_ReturnsViewResultWithDefaultModel(string errorCode)
    {
        // Act
        var result = _controller.ErrorCodePage(errorCode);

        // Assert
        Assert.That(result, Is.InstanceOf<ViewResult>());

        var viewResult = (ViewResult)result;
        Assert.That(viewResult.Model, Is.EqualTo("404"));
    }
}