using EntityService.Application.Interfaces;
using EntityService.Application.Models;
using EntityService.Domain.Interfaces;
using EntityService.Domain.Entities;

namespace EntityService.Application.Services;
public class EntityServiceImpl : IEntityService {
    private readonly IEntityRepository _repository;
    public EntityServiceImpl(IEntityRepository repository) {
        _repository = repository;
    }
    public async Task SaveEntityAsync(EntityDto entityDto) {
        var entity = new Entity {
            Id = entityDto.Id,
            OperationDate = entityDto.OperationDate,
            Amount = entityDto.Amount
        };
        await _repository.SaveAsync(entity);
    }
    public async Task<EntityDto> GetEntityByIdAsync(Guid id) {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) {
            return null;
        }
        return new EntityDto {
            Id = entity.Id,
            OperationDate = entity.OperationDate,
            Amount = entity.Amount
        };
    }
}
