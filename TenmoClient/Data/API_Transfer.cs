using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient.Data
{
    public class API_Transfer
    {
        public API_Transfer() { }
        public API_Transfer(int typeID, int statusId, int acctFrom, int acctTo, decimal amount)
        {
            this.TransferTypeId = typeID;
            this.TransferStatusId = statusId;
            this.AccountFrom = acctFrom;
            this.AccountTo = acctTo;
            this.Amount = amount;
        }
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; }
        public int TransferStatusId { get; set; }
        public int AccountFrom { get; set; }
        public int AccountTo { get; set; }
        public decimal Amount { get; set; }
    }
}
