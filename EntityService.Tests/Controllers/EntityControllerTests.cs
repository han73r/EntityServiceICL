using EntityService.Application.Interfaces;
using EntityService.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using EntityService.Web.Contollers;

namespace EntityService.Tests.Controllers;
public class EntityControllerTests {
    private readonly EntityController _controller;
    private readonly Mock<IEntityService> _serviceMock;
    private readonly Mock<ILogger<EntityController>> _loggerMock;
    public EntityControllerTests() {
        _serviceMock = new Mock<IEntityService>();
        _loggerMock = new Mock<ILogger<EntityController>>();
        _controller = new EntityController(_serviceMock.Object, _loggerMock.Object);
    }
    [Fact]
    public async Task InsertEntity_ShouldReturnBadRequest_WhenEntityDtoIsNull() {
        // Act
        var result = await _controller.InsertEntity(null);
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid entity data.", badRequestResult.Value);
    }
    [Fact]
    public async Task InsertEntity_ShouldReturnBadRequest_WhenModelStateIsInvalid() {
        // Arrange
        _controller.ModelState.AddModelError("error", "Invalid model state");
        // Act
        var result = await _controller.InsertEntity(new EntityDto());
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var modelStateDictionary = Assert.IsType<SerializableError>(badRequestResult.Value);
        Assert.True(modelStateDictionary.ContainsKey("error"));
        Assert.Contains("Invalid model state", modelStateDictionary["error"] as string[]);
    }
    [Fact]
    public async Task InsertEntity_ShouldReturnStatusCode500_WhenExceptionIsThrown() {
        // Arrange
        var entityDto = new EntityDto();
        _serviceMock.Setup(s => s.SaveEntityAsync(entityDto)).ThrowsAsync(new Exception("Test exception"));
        // Act
        var result = await _controller.InsertEntity(entityDto);
        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Internal server error: Test exception", objectResult.Value);
    }
    [Fact]
    public async Task GetEntityById_ShouldReturnBadRequest_WhenIdIsEmpty() {
        // Act
        var result = await _controller.GetEntityById(Guid.Empty);
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid entity ID.", badRequestResult.Value);
    }
    [Fact]
    public async Task GetEntityById_ShouldReturnNotFound_WhenEntityIsNotFound() {
        // Arrange
        var entityId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetEntityByIdAsync(entityId)).ReturnsAsync((EntityDto)null);
        // Act
        var result = await _controller.GetEntityById(entityId);
        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Entity not found.", notFoundResult.Value);
    }
    [Fact]
    public async Task GetEntityById_ShouldReturnStatusCode500_WhenExceptionIsThrown() {
        // Arrange
        var entityId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetEntityByIdAsync(entityId)).ThrowsAsync(new Exception("Test exception"));
        // Act
        var result = await _controller.GetEntityById(entityId);
        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Equal("Internal server error: Test exception", objectResult.Value);
    }
    [Fact]
    public async Task InsertEntity_ShouldReturnBadRequest_WhenDtoIsInvalid() {
        // Arrange
        var entityDto = new EntityDto();
        _controller.ModelState.AddModelError("Id", "The Id field is required.");
        _controller.ModelState.AddModelError("OperationDate", "The OperationDate field is required.");
        _controller.ModelState.AddModelError("Amount", "The Amount field is required.");
        // Act
        var result = await _controller.InsertEntity(entityDto);
        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}