using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AssesmentOnlineAPI.Models
{
    [Table("TransactionLog", Schema = "dbo")]
    public class TransactionLog
    {
        [Key]
        public Guid Id { get; set; }
        public int TransactionId { get; set; }
        public Guid SourceAccountID { get; set; }
        public decimal SourceNewBalance { get; set; }
        public Guid DestinationAccountID { get; set; }
        public decimal DestinationNewBalance { get; set; }
        public decimal TransferAmount { get; set; }


        public TransactionLog(int transID,Guid srcID,decimal srcNewBal, Guid destID, decimal destNewBal,decimal transferAmt)
        {
            this.TransactionId = transID;
            this.SourceAccountID = srcID;
            this.SourceNewBalance = srcNewBal;
            this.DestinationAccountID = destID;
            this.DestinationNewBalance = destNewBal;
            this.TransferAmount = transferAmt;
        }

        public TransactionLog()
        {

        }
        //[ForeignKey("TransactionID")]
        //public MasterSetting TransactionStatus { get; set; }

    }
}
