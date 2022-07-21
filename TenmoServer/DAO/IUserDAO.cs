using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{ 
    public interface IUserDAO //Might want to add things to do with accounts. Should have at least one new DAO beyond two established.
                              //Transfers for sure need one
    {
        User GetUser(string username);
        User AddUser(string username, string password);
        List<User> GetUsers();
        User GetUserFromAcctId(int acctId);
        Account GetAccountByUsername(string username);
    }
}
//add things around accounts