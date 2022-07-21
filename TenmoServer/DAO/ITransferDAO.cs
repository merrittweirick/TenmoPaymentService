using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO

    {
        Transfer CreateTransfer(Transfer newTransfer);
        List<Transfer> AllTransfersOfUser(string username);
        Transfer GetTransferById(int transferId);
        bool IncreaseBalance(int accountId, Transfer transfer);
        bool DecreaseBalance(int accountId, Transfer transfer);
        bool UpdateTransferStatus(Transfer transfer);
        Account GetAccountByUserId(int userId);

        


    }
}
