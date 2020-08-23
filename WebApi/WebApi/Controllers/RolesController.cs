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
    public class RolesController : ControllerBase
    {
        private readonly UserDbContext _context;

        public RolesController(UserDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<MessageModel<List<Role>>> GetAllRoles()
        {
            var data = await _context.Roles.Where(x=>x.IsDelete==false&&x.RoleName!= "超级管理员").ToListAsync();
            return new MessageModel<List<Role>> { 
            status=200,
            success=true,
            msg="获取成功",
            response =data
            };
        }
        /// <summary>
        /// 查询角色列表（分页）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Roles
        [HttpGet]
        public async Task<MessageModel<PageModel<Role>>> GetRoles(int page = 1, int pagesize = 5)
        {
            var data = await _context.Roles.ToListAsync();
            var dataCount = data.Count;

            data = data.Skip((page-1) * pagesize).Take(pagesize).OrderBy(x => x.Id).ToList();
            return new MessageModel<PageModel<Role>>()
            {
                msg = "获取成功",
                success = true,
                response = new PageModel<Role>()
                {
                    page = page,
                    dataCount = dataCount,
                    data = data,
                    pageCount = dataCount / pagesize,
                    PageSize = pagesize
                }
            };
        }
        /// <summary>
        /// 查询角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // PUT: api/Roles/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, Role role)
        {
            if (id != role.Id)
            {
                return BadRequest();
            }

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
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
        /// 添加角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: api/Roles
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRole", new { id = role.Id }, role);
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Role>> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return role;
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
