using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    /// <summary>
    /// This class must ONLY have endpoints related to authentication. 
    /// Other endpoints go in other NEW controllers.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ITokenGenerator tokenGenerator;
        private readonly IPasswordHasher passwordHasher;
        private readonly IUserDAO userDAO;

        public LoginController(ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher, IUserDAO userDAO)
        {
            this.tokenGenerator = tokenGenerator;
            this.passwordHasher = passwordHasher;
            this.userDAO = userDAO;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Authenticate(LoginUser userParam)
        {
            // Get the user by username
            User user = userDAO.GetUser(userParam.Username);

            // If we found a user and the password hash matches
            if (user != null && passwordHasher.VerifyHashMatch(user.PasswordHash, userParam.Password, user.Salt))
            {
                // Create an authentication token
                string token = tokenGenerator.GenerateToken(user.UserId, user.Username);

                // Create a ReturnUser object to return to the client
                ReturnUser retUser = new ReturnUser() { 
                    UserId = user.UserId, 
                    Username = user.Username, 
                    Token = token 
                };

                // Switch to 200 OK
                return Ok(retUser);
            }

            return BadRequest(new { message = "Username or password is incorrect" });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register(LoginUser userParam)
        {
            User existingUser = userDAO.GetUser(userParam.Username);
            if (existingUser != null)
            {
                return Conflict(new { message = "Username already taken. Please choose a different username." });
            }

            User user = userDAO.AddUser(userParam.Username, userParam.Password);
            if (user == null)
            {
                return BadRequest(new { message = "An error occurred and user was not created." });
            }
            else
            {
                return Created(user.Username, null); //values aren't read on client
            }
        }
    }
}
