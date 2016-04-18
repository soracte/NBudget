using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace NBudget.Controllers.ApiControllers
{
    [RoutePrefix("api/roles")]
    public class RolesController : BaseApiController
    {
        [Route("", Name = "GetAllRoles")]
        public IHttpActionResult GetAllRoles()
        {
            var roles = RoleManager.Roles;
            var retval = roles.Select(r => new { Name = r.Name });
            return Ok(retval);
        }

        [Route("{id:guid}", Name = "GetRoleById")]
        public async Task<IHttpActionResult> GetRole(string Id)
        {
            var role = await RoleManager.FindByIdAsync(Id);

            if (role != null)
            {
                return Ok(new { Id = role.Id, Name = role.Name });
            }

            return NotFound();
        }
        [Route("create")]
        public async Task<IHttpActionResult> Create(string roleName)
        {
            var role = new IdentityRole { Name = roleName };
            var result = await RoleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                return InternalServerError(); 
            }
            Uri locationHeader = new Uri(Url.Link("GetRoleById", new { id = role.Id }));

            return Created(locationHeader, new { Name = role.Name });

        }
    }
}
