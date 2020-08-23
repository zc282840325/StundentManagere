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
    public class PermissionsController : ControllerBase
    {
        private readonly UserDbContext _context;

        public PermissionsController(UserDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 查询权限列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Permissions
        [HttpGet]
        public async Task<MessageModel<PageModel<Permission>>> GetUsers(int page =1, int pagesize = 5)
        {
            var data = await _context.Permissions.ToListAsync();
            var dataCount = data.Count;
            page -= 1;
            data = data.Skip(page * pagesize).Take(pagesize).OrderBy(x => x.Id).ToList();
            return new MessageModel<PageModel<Permission>>()
            {
                msg = "获取成功",
                success = true,
                response = new PageModel<Permission>()
                {
                    page = page-1,
                    dataCount = dataCount,
                    data = data,
                    pageCount = dataCount / pagesize,
                    PageSize = pagesize
                }
            };
        }
        /// <summary>
        /// 查询权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Permissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Permission>> GetPermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);

            if (permission == null)
            {
                return NotFound();
            }

            return permission;
        }
        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // PUT: api/Permissions/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPermission(int id, Permission permission)
        {
            if (id != permission.Id)
            {
                return BadRequest();
            }

            _context.Entry(permission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PermissionExists(id))
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
        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: api/Permissions
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Permission>> PostPermission(Permission permission)
        {
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPermission", new { id = permission.Id }, permission);
        }
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Permissions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Permission>> DeletePermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return permission;
        }

        private bool PermissionExists(int id)
        {
            return _context.Permissions.Any(e => e.Id == id);
        }
    }
}
