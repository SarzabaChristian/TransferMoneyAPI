using AssesmentOnlineAPI.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AssesmentOnlineAPI.Models
{
    [Table("Users", Schema = "dbo")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; } = string.Empty;
        [Column(TypeName ="decimal(18,2)")]
        public decimal Balance { get; set; }


        
        public static implicit operator User(userForRegisterDTO userRegisterDTO)
        {
            return new User
            {
                Username = userRegisterDTO.Username,
                FirstName=userRegisterDTO.FirstName,
                LastName=userRegisterDTO.LastName,
                MiddleName=userRegisterDTO.MiddleName,
                Balance = userRegisterDTO.InitialBalance
            };
        }
    }
}
