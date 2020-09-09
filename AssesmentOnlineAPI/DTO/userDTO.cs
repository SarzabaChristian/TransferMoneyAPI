using AssesmentOnlineAPI.Models;
using System;

namespace AssesmentOnlineAPI.DTO
{
    public class userDTO
    {
        public Guid UserID { get; set; }
        public string FullName { get; set; }
        public decimal Balance { get; set; }

        public static implicit operator userDTO(User activeUser)
        {
            return new userDTO
            {
                UserID = activeUser.Id,
                FullName = (activeUser.LastName + ", " + activeUser.FirstName + " " + activeUser.MiddleName),
                Balance= activeUser.Balance
            };
        }
    }
}
