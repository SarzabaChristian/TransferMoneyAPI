using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssesmentOnlineAPI.Data;
using AssesmentOnlineAPI.Models;
using Microsoft.EntityFrameworkCore;
using AssesmentOnlineAPI.Helpers;
using Microsoft.EntityFrameworkCore.Storage;

namespace AssesmentOnlineAPI.Services
{
    public class UserServices : IUserServices
    {
        private readonly TransferAccountDBContext _context;
        public UserServices(TransferAccountDBContext context)
        {
            _context = context;
        }

        public async Task<User> GetUser(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> isAvailableToTransfer(Guid userId, decimal amountToBeTransfer)
        {
            try
            {
                var currentUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                return amountToBeTransfer > currentUser.Balance ? false : true;
            }
            catch (Exception ex)
            {
                var a = ex.Message;
                throw;
            }         
        }

        public async Task<TransactionLog> TransferMoney(Guid sourceAccountID, Guid destinationAccountID, decimal transferAmount)
        {
            decimal destNewBalance, srcNewBalance;
           
            if (!TransferMoneyToUsers(sourceAccountID, destinationAccountID, transferAmount,out destNewBalance,out srcNewBalance))
            return null;

            var newTransaction = new TransactionLog();
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {            
                newTransaction.SourceAccountID = sourceAccountID;
                newTransaction.DestinationAccountID = destinationAccountID;
                newTransaction.TransactionId = (int)GlobalEnum.TransactionType.Transfer;
                newTransaction.TransferAmount = transferAmount;
                newTransaction.DestinationNewBalance = destNewBalance;
                newTransaction.SourceNewBalance = srcNewBalance;

                _context.TransactionLogs.Add(newTransaction);
                await _context.SaveChangesAsync();
                transaction.Commit();
            }





            return newTransaction;
        }

        private User GetUserByID(Guid id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }
        private bool TransferMoneyToUsers(Guid sourceAccountID, Guid destinationAccountID, decimal transferAmount,out decimal destinationNewBalance,out decimal sourceNewBalance)
        {
            var isSaved = false;
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                var currentUser = GetUserByID(sourceAccountID);
                var destinationUser = GetUserByID(destinationAccountID);

                destinationUser.Balance += transferAmount;
                currentUser.Balance -= transferAmount;

                _context.Entry(currentUser).Property(x => x.Balance).IsModified = true;
                _context.Entry(destinationUser).Property(x => x.Balance).IsModified = true;
                destinationNewBalance = destinationUser.Balance;
                sourceNewBalance = currentUser.Balance;
                isSaved = _context.SaveChanges() > 0;
                transaction.Commit();
            }            
            if (!isSaved)
                return false;          

            return isSaved;
        }
    }
}
