using AssesmentOnlineAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssesmentOnlineAPI.Services
{
    public interface IAuthServices
    {
        Task<User> Register(User newUser,string password);
        Task<User> Login(string username, string password);
        Task<bool> isExistingUser(string username);
    }
}
