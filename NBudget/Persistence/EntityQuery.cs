using NBudget.Models;
using System.Data.Entity;
using System.Linq;

namespace NBudget.Persistence
{
    public class EntityQuery<T> where T : OwnedEntity
    {
        public IQueryable<T> EntitiesOfUser(DbSet<T> entities, string userId)
        {
            return entities.Where(cat => cat.Owner.Id == userId);
        }

    }
}