using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssesmentOnlineAPI.Data;
using AssesmentOnlineAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AssesmentOnlineAPI.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly TransferAccountDBContext _context;
        public AuthServices(TransferAccountDBContext context)
        {
            _context = context;
        }

        public async Task<bool> isExistingUser(string username)
        {
            var result = false;
            try
            {
                result=await _context.Users.AnyAsync(x => x.Username == username);
            }
            catch (Exception ex)
            {
                var a = ex.Message.ToString();
                throw;
            }
            return result;
        }

        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
                return null;

            if (!DecryptPassword(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public async Task<User> Register(User newUser, string password)
        {
            byte[] passwordHash, passwordSalt;
            EncryptPassword(password, out passwordHash, out passwordSalt);
            newUser.PasswordHash = passwordHash;
            newUser.PasswordSalt = passwordSalt;
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        private void EncryptPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool DecryptPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (passwordHash[i] != computedHash[i])
                        return false;
                }
            }

            return true;
        }
    }
}
