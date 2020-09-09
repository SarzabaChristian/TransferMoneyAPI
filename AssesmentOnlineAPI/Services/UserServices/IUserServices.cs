using AssesmentOnlineAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssesmentOnlineAPI.Services
{
    public interface IUserServices
    {
        Task<User> GetUser(Guid id);
        Task<bool> isAvailableToTransfer(Guid userId,decimal amountToBeTransfer);
        Task<TransactionLog> TransferMoney(Guid sourceAccountID, Guid destinationAccountID, decimal transferAmount);
    }
}
