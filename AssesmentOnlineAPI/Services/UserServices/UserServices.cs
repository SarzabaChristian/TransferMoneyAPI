using System;
using System.Linq;
using System.Threading.Tasks;
using AssesmentOnlineAPI.Data;
using AssesmentOnlineAPI.Models;
using Microsoft.EntityFrameworkCore;
using AssesmentOnlineAPI.Helpers;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using AssesmentOnlineAPI.Helpers;
using System.Diagnostics;

namespace AssesmentOnlineAPI.Services
{
    public class UserServices : IUserServices
    {
        //private object processLock = new object();
        AutoResetEvent threadHandler = new AutoResetEvent(false);
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
            var newTransaction = new TransactionLog();
            ThreadControl.threads.Add(Task.Run(() => TransferMoneyToUsers(sourceAccountID, destinationAccountID, transferAmount, ref newTransaction)));
            Task.WaitAll(ThreadControl.threads.ToArray());
            return newTransaction;
        }
        private User GetUserByID(Guid id)
        {
            return _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == id);
        }
        //
        private void TransferMoneyToUsers(Guid sourceAccountID, Guid destinationAccountID, decimal transferAmount, ref TransactionLog transactionLog)
        {
            var sourceUser = new User();
            var destinationUser = new User();
            decimal srcNewBal = 0;
            decimal dstNewBal = 0;
            Monitor.Enter(ThreadControl.obj);
            try
            {
                ThreadControl.threadCount += 1;
                using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        sourceUser = GetUserByID(sourceAccountID);
                        destinationUser = GetUserByID(destinationAccountID);
                        srcNewBal = sourceUser.Balance - transferAmount;
                        dstNewBal = destinationUser.Balance + transferAmount;
                        UpdateBalance(sourceUser, srcNewBal);
                        UpdateBalance(destinationUser, dstNewBal);
                        if (sourceUser.Balance >= transferAmount || sourceUser.Balance == 0)
                        {
                            var newTransaction = new TransactionLog();
                            newTransaction.SourceAccountID = sourceUser.Id;
                            newTransaction.DestinationAccountID = destinationUser.Id;
                            newTransaction.TransactionId = (int)GlobalEnum.TransactionType.Transfer;
                            newTransaction.TransferAmount = transferAmount;
                            newTransaction.DestinationNewBalance = srcNewBal;
                            newTransaction.SourceNewBalance = dstNewBal;
                            _context.TransactionLogs.Add(newTransaction);
                            _context.SaveChanges();
                            transactionLog = newTransaction;
                        }
                        else
                        {
                            return;
                        }
                        transaction.Commit();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Failed!");
                    }
                    transaction.Dispose();
                }
            }
            finally
            {
                Monitor.Exit(ThreadControl.obj);
            }
        }
        private void UpdateBalance(User user, decimal amount)
        {
            user.Balance = amount;
            _context.Entry(user).Property(x => x.Balance).IsModified = true;
        }
    }
}
