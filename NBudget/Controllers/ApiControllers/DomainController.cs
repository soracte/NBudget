using NBudget.Models;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Data.Entity;

namespace NBudget.Controllers.ApiControllers
{
    public class DomainController<T> : BaseApiController where T : OwnedEntity
    {
        protected IQueryable<T> getAccessibleEntities(DbSet<T> entities)
        {
            return null;
        }
        protected IQueryable<T> getAccessibleEntities(DbSet<T> entities, string userId)
        {
            var currentUserId = User.Identity.GetUserId();
            if (userId != currentUserId)
            {
                var inviterUserIds = UserManager.FindById(currentUserId).Inviters.Select(inv => inv.Id);

                if (!inviterUserIds.Contains(userId))
                {
                    return Enumerable.Empty<T>().AsQueryable();
                }
            }

            return entities.Where(cat => cat.Owner.Id == userId);
        }
    }
}
