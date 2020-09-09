using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AssesmentOnlineAPI.DTO
{
    public class transferMoneyDTO
    {
        [Required(ErrorMessage = "Must Supply value in SourceAccountID")]
        public Guid SourceAccountID { get; set; }
        [Required(ErrorMessage = "Must Supply value in DestinationAccountID")]
        public Guid DestinationAccountID { get; set; }
        [Required(ErrorMessage = "Must Supply value in Amount")]
        [Range(0.1,double.MaxValue, ErrorMessage ="Transfer amount must greater than 0")]
        public decimal Amount { get; set; }
    }
}
