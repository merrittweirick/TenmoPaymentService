using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public Transfer CreateTransfer(Transfer newTransfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                const string sql = "Insert into transfers " +
                    "(transfer_type_id, transfer_status_id," +
                    " account_from, account_to, amount)" +
                    "Values (@typeId,@statusId,@acctFromId,@acctToId,@amount);"+
                    "Select @@IDENTITY;";
                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@typeId", newTransfer.TransferTypeId);
                command.Parameters.AddWithValue("@statusId", newTransfer.TransferStatusId);
                command.Parameters.AddWithValue("@acctFromId", newTransfer.AccountFrom);
                command.Parameters.AddWithValue("@acctToId", newTransfer.AccountTo);
                command.Parameters.AddWithValue("@amount", newTransfer.Amount);

                int id = Convert.ToInt32(command.ExecuteScalar());

                newTransfer.TransferId = id;
                return newTransfer;
            }
        }

        public List<Transfer> AllTransfersOfUser(string username)
        {
            List<Transfer> transfers = new List<Transfer>();

            const string sql = "Select transfer_id, transfer_type_id, transfer_status_id, account_from, account_to, amount " +
                "From transfers t " +
                "inner join accounts a on a.account_id = t.account_from " +
                "inner join accounts ac on ac.account_id = t.account_to " +
                "inner join users u on u.user_id = a.user_id inner join users us on us.user_id = ac.user_id " +
                "WHERE u.username = @username1 or us.username = @username2";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@username1", username);
                command.Parameters.AddWithValue("@username2", username);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Transfer transfer = GetTransferFromDataReader(reader);
                    transfers.Add(transfer);
                }

                return transfers;
            }

        }
        public Transfer GetTransferFromDataReader(SqlDataReader reader)
        {
            return new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TransferTypeId = Convert.ToInt32(reader["transfer_type_id"]),
                TransferStatusId = Convert.ToInt32(reader["transfer_status_id"]),
                AccountFrom = Convert.ToInt32(reader["account_from"]),
                AccountTo = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };
        }

        public Boolean DecreaseBalance(int accountId, Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("UPDATE accounts " +
                    "SET balance -= @amount WHERE account_id = @accountId", conn);
                command.Parameters.AddWithValue("@accountId", accountId);
                command.Parameters.AddWithValue("@amount", transfer.Amount);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return true;
                }
            }
            return false;
        }
        private Account GetAccountFromReader(SqlDataReader reader)
        {
            return new Account()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                AccountId = Convert.ToInt32(reader["account_id"]),
                Balance = Convert.ToDecimal(reader["balance"]),
            };
        }

        public Boolean IncreaseBalance(int accountToId, Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("UPDATE accounts SET balance += @amount " +
                    "WHERE account_id = @accountId", conn);
                command.Parameters.AddWithValue("@accountId", accountToId);
                command.Parameters.AddWithValue("@amount", transfer.Amount);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public Transfer GetTransferById(int transferId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand command = new SqlCommand("SELECT transfer_id, transfer_type_id, " +
                    "transfer_status_id, " +
                    "account_from, account_to, amount " +
                    "FROM transfers " +
                    "WHERE transfer_id = @transferId", conn);
                command.Parameters.AddWithValue("@transferId", transferId);

                SqlDataReader reader = command.ExecuteReader();

                if(reader.HasRows && reader.Read())
                {
                    Transfer transfer = GetTransferFromDataReader(reader);
                    return transfer;
                }
            }
            return null;
        }

        public bool UpdateTransferStatus(Transfer transfer)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE transfers " +
                    "SET transfer_status_id = @transferStatusId " +
                    "WHERE transfer_id = @transferId", conn);
                cmd.Parameters.AddWithValue("transferStatusId", transfer.TransferStatusId);
                cmd.Parameters.AddWithValue("@transferId", transfer.TransferId);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public Account GetAccountByUserId(int userId)
        {
            Account returnAccount = new Account();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("Select a.user_id, a.account_id, balance from accounts a inner join users u on a.user_id = u.user_id where u.user_id= @user_id", conn);
                cmd.Parameters.AddWithValue("@user_id", userId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows && reader.Read())
                {
                    returnAccount = GetAccountFromReader(reader);
                }
            }

            return returnAccount;
        }
    }
}
