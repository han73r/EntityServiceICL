using EntityService.Domain.Entities;
using EntityService.Domain.Interfaces;

namespace EntityService.Infrastructure.Repositories;
public class InMemoryEntityRepository : IEntityRepository {
    private readonly List<Entity> _entities = new List<Entity>();
    public async Task SaveAsync(Entity entity) { 
        _entities.Add(entity);
        await Task.CompletedTask;
    }
    public async Task<Entity> GetByIdAsync(Guid id) {
        return await Task.FromResult(_entities.FirstOrDefault(e => e.Id == id));
    }
}