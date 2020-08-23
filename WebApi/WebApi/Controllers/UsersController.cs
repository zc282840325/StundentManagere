using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Dto;
using WebApi.EF;
using WebApi.Model;
using WebApi.Tool;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Permissions.Name)]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(UserDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        /// <summary>
        ///  获取用户列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>

        // GET: api/Users
        [HttpGet]
        public async Task<MessageModel<PageModel<User>>> GetUsers(int page=1,int pagesize=5)
        {
            var data= await _context.Users.ToListAsync();
            var dataCount = data.Count;
            data = data.Skip((page-1) * pagesize).Take(pagesize).OrderBy(x => x.Id).ToList();
            return new MessageModel<PageModel<User>>()
            {
                msg = "获取成功",
                success = true,
                response = new PageModel<User>()
                {
                    page = page,
                    dataCount = dataCount,
                    data = data,
                    pageCount = dataCount / pagesize,
                    PageSize= pagesize
                }
            };
        }
        /// <summary>
        /// 查询用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<MessageModel<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            string msg = string.Empty;
            if (user == null)
            {
                msg = "未找到";
            }
            else
            {
                msg = "查询成功";
            }

            return new MessageModel<User>()
            {
                status=200,
                msg = msg,
                success = true,
                response = user
            };
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
           
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;
            user.UserPwd=MD5Helper.MD5Encrypt32(user.UserPwd);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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
        /// 添加用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
