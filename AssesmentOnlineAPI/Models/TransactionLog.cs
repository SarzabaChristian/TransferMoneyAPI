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
        [Column(TypeName = "decimal(18,2)")]
        public decimal SourceNewBalance { get; set; }
        public Guid DestinationAccountID { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DestinationNewBalance { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TransferAmount { get; set; }
        
        //[ForeignKey("TransactionID")]
        //public MasterSetting TransactionStatus { get; set; }

    }
}
