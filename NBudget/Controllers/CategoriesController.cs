﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using NBudget.Models;
using Microsoft.AspNet.Identity;
using NBudget.Controllers.ApiControllers;
using NBudget.Persistence;
using NBudget.Attributes;

namespace NBudget.Controllers
{
    [RoutePrefix("api/Categories")]
    public class CategoriesController : BaseApiController 
    {
        private EntityQuery<Category> cats = new EntityQuery<Category>();

        // GET: api/Categories
        [Route("{userId}")]
        [AnotherUserAuthorize]
        public IHttpActionResult GetCategories(string userId)
        {
            var currentUserId = User.Identity.GetUserId();

            var categories = cats.EntitiesOfUser(db.Categories, userId).Select(cat => new { Id = cat.Id, Name = cat.Name });
            return Ok(categories);
        }

        // GET: api/Categories/5
        [ResponseType(typeof(Category))]
        [Route("{userId}/{id}")]
        [AnotherUserAuthorize]
        public IHttpActionResult GetCategory(string userId, int id)
        {
            Category category = cats.EntitiesOfUser(db.Categories, userId).SingleOrDefault(cat => cat.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(new { Id = category.Id, Name = category.Name });
        }

        // PUT: api/Categories/5
        [ResponseType(typeof(void))]
        [Route("{userId}/{id}")]
        public IHttpActionResult PutCategory(string userId, int id, CategoryDTO category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Category updated = cats.EntitiesOfUser(db.Categories, userId).SingleOrDefault(cat => cat.Id == id);
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
        [Route("{userId}", Name = "PostCategory")]
        public IHttpActionResult PostCategory(string userId, [FromBody] CategoryDTO category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = UserManager.FindById(User.Identity.GetUserId());
            Category added = new Category { Name = category.Name, Owner = currentUser };

            db.Categories.Add(added);
            db.SaveChanges();

            return CreatedAtRoute("PostCategory", new { id = added.Id }, category);
        }

        // DELETE: api/Categories/5
        [ResponseType(typeof(Category))]
        [Route("{userId}/{id}")]
        public IHttpActionResult DeleteCategory(string userId, int id)
        {
            Category category = cats.EntitiesOfUser(db.Categories, userId).SingleOrDefault(cat => cat.Id == id);
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