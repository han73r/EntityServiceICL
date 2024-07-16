using EntityService.Domain.Entities;

namespace EntityService.Domain.Interfaces;
public interface IEntityRepository {
    Task SaveAsync(Entity entity);
    Task<Entity> GetByIdAsync(Guid id);
}
