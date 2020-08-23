using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.EF;
using WebApi.Model;
using WebApi.Tool;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class RolePermissionsController : ControllerBase
    {
        private readonly UserDbContext _context;

        public RolePermissionsController(UserDbContext context)
        {
            _context = context;
        }

        // GET: api/RolePermissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolePermission>>> GetRolePermissions()
        {
            return await _context.RolePermissions.ToListAsync();
        }

        // GET: api/RolePermissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RolePermission>> GetRolePermission(int id)
        {
            var rolePermission = await _context.RolePermissions.FindAsync(id);

            if (rolePermission == null)
            {
                return NotFound();
            }

            return rolePermission;
        }

        // PUT: api/RolePermissions/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRolePermission(int id, RolePermission rolePermission)
        {
            if (id != rolePermission.Id)
            {
                return BadRequest();
            }

            _context.Entry(rolePermission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolePermissionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RolePermissions
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RolePermission>> PostRolePermission(RolePermission rolePermission)
        {
            _context.RolePermissions.Add(rolePermission);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRolePermission", new { id = rolePermission.Id }, rolePermission);
        }

        // DELETE: api/RolePermissions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RolePermission>> DeleteRolePermission(int id)
        {
            var rolePermission = await _context.RolePermissions.FindAsync(id);
            if (rolePermission == null)
            {
                return NotFound();
            }

            _context.RolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync();

            return rolePermission;
        }

        private bool RolePermissionExists(int id)
        {
            return _context.RolePermissions.Any(e => e.Id == id);
        }
    }
}
