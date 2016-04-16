using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using NBudget.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NBudget.Controllers.ApiControllers;

namespace NBudget.Controllers
{
    public class CategoriesController : BaseApiController
    {
        private NBudgetContext db = new NBudgetContext();

        // GET: api/Categories
        public IHttpActionResult GetCategories()
        {
            var currentUserId = User.Identity.GetUserId();
            var categories = db.Categories.Where(cat => cat.Owner.Id == currentUserId).Select(cat => new { Id = cat.Id, Name = cat.Name });
            return Ok(categories);
        }

        // GET: api/Categories/5
        [ResponseType(typeof(Category))]
        public IHttpActionResult GetCategory(int id)
        {
            Category category = db.Categories.Single(cat => cat.Id == id && cat.Owner.Id == User.Identity.GetUserId());
            if (category == null)
            {
                return NotFound();
            }

            return Ok(new { Id = category.Id, Name = category.Name });
        }

        // PUT: api/Categories/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCategory(int id, CategoryDTO category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Category updated = db.Categories.Single(cat => cat.Id == id && cat.Owner.Id == User.Identity.GetUserId());
            if (updated == null)
            {
                return NotFound();
            }

            updated.Id = id;
            updated.Name = category.Name;

            db.Entry(updated).State = EntityState.Modified;

            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Categories
        [ResponseType(typeof(Category))]
        public IHttpActionResult PostCategory(CategoryDTO category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.Identity.GetUserId();
            IdentityUser currentUser = db.Users.Find(currentUserId);
            Category added = new Category { Name = category.Name, Owner = currentUser };

            db.Categories.Add(added);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = added.Id }, category);
        }

        // DELETE: api/Categories/5
        [ResponseType(typeof(Category))]
        public IHttpActionResult DeleteCategory(int id)
        {
            Category category = db.Categories.Single(cat => cat.Id == id && cat.Owner.Id == User.Identity.GetUserId());
            if (category == null)
            {
                return NotFound();
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            return Ok(category);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}