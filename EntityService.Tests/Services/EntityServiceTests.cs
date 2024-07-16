using Xunit;
using Moq;
using EntityService.Application.Models;
using EntityService.Application.Services;
using EntityService.Domain.Interfaces;
using EntityService.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace EntityService.Tests.Services;
public class EntityServiceTests {
    private readonly Mock<IEntityRepository> _repositoryMock;
    private readonly EntityServiceImpl _entityService;
    public EntityServiceTests() {
        _repositoryMock = new Mock<IEntityRepository>();
        _entityService = new EntityServiceImpl(_repositoryMock.Object);
    }
    private void SetupRepositoryToSaveEntity() {
        _repositoryMock.Setup(r => r.SaveAsync(It.IsAny<Entity>()))
                       .Returns(Task.CompletedTask);
    }
    private void SetupRepositoryToGetEntityById(Guid entityId, Entity entity) {
        _repositoryMock.Setup(r => r.GetByIdAsync(entityId))
                       .ReturnsAsync(entity);
    }
    private void SetupRepositoryToReturnNullForEntityById(Guid entityId) {
        _repositoryMock.Setup(r => r.GetByIdAsync(entityId))
                       .ReturnsAsync((Entity)null);
    }
    private EntityDto CreateEntityDto(Guid id) {
        return new EntityDto {
            Id = id,
            OperationDate = DateTime.UtcNow,
            Amount = 100.00m
        };
    }
    private Entity CreateEntityFromDto(EntityDto entityDto) {
        return new Entity {
            Id = entityDto.Id,
            OperationDate = entityDto.OperationDate,
            Amount = entityDto.Amount
        };
    }
    [Fact]
    public async Task SaveEntityAsync_ShouldSaveEntity() {
        // Arrange
        var entityDto = CreateEntityDto(Guid.NewGuid());
        SetupRepositoryToSaveEntity();

        // Act
        await _entityService.SaveEntityAsync(entityDto);
        // Assert
        _repositoryMock.Verify(r => r.SaveAsync(It.Is<Entity>(e =>
            e.Id == entityDto.Id &&
            e.OperationDate == entityDto.OperationDate &&
            e.Amount == entityDto.Amount)), Times.Once);
    }
    [Fact]
    public async Task GetEntityByIdAsync_ShouldReturnEntity() {
        // Arrange
        var entityId = Guid.NewGuid();
        var entity = new Entity {
            Id = entityId,
            OperationDate = DateTime.UtcNow,
            Amount = 100.00m
        };
        SetupRepositoryToGetEntityById(entityId, entity);
        // Act
        var result = await _entityService.GetEntityByIdAsync(entityId);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.OperationDate, result.OperationDate);
        Assert.Equal(entity.Amount, result.Amount);
    }
    [Fact]
    public async Task SaveAndRetrieveEntity_ShouldWorkCorrectly() {
        // Arrange
        var entityDto = CreateEntityDto(Guid.NewGuid());
        var entity = CreateEntityFromDto(entityDto);
        SetupRepositoryToSaveEntity();
        SetupRepositoryToGetEntityById(entityDto.Id, entity);
        // Act
        await _entityService.SaveEntityAsync(entityDto);
        var retrievedEntity = await _entityService.GetEntityByIdAsync(entityDto.Id);
        // Assert
        Assert.NotNull(retrievedEntity);
        Assert.Equal(entityDto.Id, retrievedEntity.Id);
        Assert.Equal(entityDto.OperationDate, retrievedEntity.OperationDate);
        Assert.Equal(entityDto.Amount, retrievedEntity.Amount);
    }
    [Fact]
    public async Task GetEntityByIdAsync_ShouldReturnNull_WhenEntityNotFound() {
        // Arrange
        var entityId = Guid.NewGuid();
        SetupRepositoryToReturnNullForEntityById(entityId);
        // Act
        var result = await _entityService.GetEntityByIdAsync(entityId);
        // Assert
        Assert.Null(result);
    }
}