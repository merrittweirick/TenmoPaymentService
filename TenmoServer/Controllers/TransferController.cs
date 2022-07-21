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
    public class TransferController : ControllerBase
    {
        private readonly ITransferDAO transferDAO;

        public TransferController(ITransferDAO transferDAO)
        {
            this.transferDAO = transferDAO;
        }

        [HttpGet("transfer")]
        [Authorize]
        public ActionResult GetAllTransfersOfUser()
        {
            string username = User.Identity.Name;

            List<Transfer> transferHistory = transferDAO.AllTransfersOfUser(username);

            if (transferHistory == null)
            {
                return NotFound("Could not find and transfers matching username " + username);
            }
            return Ok(transferHistory);
        }


        [HttpGet("transfer/{transferId}")]
        //[Authorize]

        public ActionResult GetTransferById(int transferId)
        {
            Transfer transfer = transferDAO.GetTransferById(transferId);

            if (transfer == null)
            {
                return BadRequest("Transfer Is fucking null");
            }

            return Ok(transfer);
        }

        [HttpPost("transfer")]
        public ActionResult CreateTransfer(Transfer transfer)
        {
            Transfer newTransfer = transferDAO.CreateTransfer(transfer);
            if (newTransfer == null)
            {
                return BadRequest("Your entry was invalid");
            }
            return Ok(newTransfer);
        }

        [HttpGet("transfer/account/{userId}")]
        [Authorize]
        public ActionResult GetUsersAccount(int userId)
        {
            Account account = transferDAO.GetAccountByUserId(userId);

            if (account == null)
            {
                return NotFound("Could not find account match the User ID: " + userId);
            }
            return Ok(account);
        }

        [HttpPut("transfer/to/{accountId}")]
        //[Authorize]
        public ActionResult IncreaseBalance(int accountId, Transfer transfer)
        {
            bool updated = transferDAO.IncreaseBalance(accountId, transfer);

            if (updated == false)
            {
                return BadRequest("Didn't work");
            }

            return Ok();
        }

        [HttpPut("transfer/from/{accountId}")]
        //[Authorize]
        public ActionResult DecreaseBalance(int accountId, Transfer transfer)
        {
            bool updated = transferDAO.DecreaseBalance(accountId, transfer);

            if (updated == false)
            {
                return BadRequest("Didn't work");
            }

            return Ok();
        }

        [HttpPut("transfer/update")]
        //[Authorize]
        public ActionResult UpdateTransferStatus(Transfer transfer)
        {
            bool updated = transferDAO.UpdateTransferStatus(transfer);

            if(updated == false)
            {
                return BadRequest("Unable to update status");
            }

            return Ok();
        }

    }
}
