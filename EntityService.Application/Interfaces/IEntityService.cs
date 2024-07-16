using EntityService.Application.Models;

namespace EntityService.Application.Interfaces;
public interface IEntityService {
    Task SaveEntityAsync(EntityDto entityDto);
    Task<EntityDto> GetEntityByIdAsync(Guid id);
}
