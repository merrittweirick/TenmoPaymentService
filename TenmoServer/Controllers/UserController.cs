using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Controllers;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TenmoServer.Controllers
{
    [Route("/")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserDAO userDAO;

        public UserController(IUserDAO userDAO)
        {
            this.userDAO = userDAO;
        }

        private int LoggedInUserId
        {
            get
            {
                Claim idClaim = User.FindFirst("sub");
                if (idClaim == null)
                {
                    // User is not logged in
                    return -1;
                }
                else
                {
                    // User is logged in. Their subject (sub) claim is their ID
                    return int.Parse(idClaim.Value);
                }
            }
        }



        [HttpGet("user")]
        [Authorize]
        public ActionResult GetUser()
        {
            string username = User.Identity.Name; ;
            
            User user = userDAO.GetUser(username);

            if(user == null)
            {
                return NotFound("Could not find user " + username);
            }
            return Ok(user);
        }

        [HttpGet("user/all")]
        //[Authorize]
        public ActionResult GetAllUsers()
        {
            List<User> users = userDAO.GetUsers();
            return Ok(users);
        }

        [HttpPost("user")]
        public ActionResult AddUser(User user)
        {
            string username = User.Identity.Name;

            int id = LoggedInUserId;

            user.UserId = id;

            User createdUser = userDAO.AddUser(user.Username, user.PasswordHash);

            return Ok(createdUser);
        }

        [HttpGet("user/account/{username}")]
        public ActionResult GetAccountFromUsername(string username)
        {
            Account usersAccount = userDAO.GetAccountByUsername(username);

            if(usersAccount == null)
            {
                return NotFound("Could not find account with matching affiliated username of: "+ username);
            }

            return Ok(usersAccount);
        }
        [HttpGet("user/{accountId}")]
        public ActionResult GetUserFromAccountId(int accountId)
        {
            User user = userDAO.GetUserFromAcctId(accountId);

            if(user == null)
            {
                return NotFound("Could not find user with matching account number: " + accountId);
            }
            return Ok(user);
        }


    }
}
