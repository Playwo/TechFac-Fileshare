using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fileshare.Extensions;
using Fileshare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fileshare.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly FileshareContext DbContext;
        private readonly Random Random;
        private readonly ILogger Logger;

        public UserController(FileshareContext dbContext, Random random, ILogger<UserController> logger)
        {
            DbContext = dbContext;
            Random = random;
            Logger = logger;
        }

        //user/create
        [HttpPost("create")]
        [Authorize(AuthenticationSchemes = "ApiKey")]
        public async Task<ActionResult<User>> CreateUserAsync([FromForm]string username, [FromForm]string password)
        {
            bool nameUsed = await DbContext.Users.AnyAsync(x => x.Username == username);

            if (nameUsed)
            {
                Logger.LogInformation($"Someone tried to create a user but the name is already being used: {username}");
                return Conflict("Name already used");
            }

            string token = Random.NextString(20);
            var user = new User(username, password, token);

            DbContext.Add(user);
            await DbContext.SaveChangesAsync();

            Logger.LogInformation($"Someone created a user: {username} [{user.Id}]");

            return user;
        }

        [HttpGet("get")]
        [Authorize(AuthenticationSchemes = "Basic")]
        public async Task<ActionResult<User>> GetUserAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue("UserId"));

            var user = await DbContext.Users.Where(x => x.Id == userId)
                                            .FirstOrDefaultAsync();

            return Ok(user);
        }
    }
}
