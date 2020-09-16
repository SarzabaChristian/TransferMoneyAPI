using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AssesmentOnlineAPI.Data;
using AssesmentOnlineAPI.DTO;
using System;
using System.Threading;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AssesmentOnlineAPI.Test
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {                      
                        services.RemoveAll(typeof(TransferAccountDBContext));
                        services.AddDbContext<TransferAccountDBContext>(options => { options.UseInMemoryDatabase("TestDb")
                                                                                            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)); });
                    });
                });

            TestClient = appFactory.CreateClient();       
        }
        protected void InvalidaAuthentication()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", "invalid");
        }
        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            var newUser=new userForRegisterDTO
            {
                Username = "sampleuser",
                FirstName = "sample",
                LastName = "sample",
                Password = "password",
                InitialBalance = 10
            };
            var newUserRegistered=await CreateUser(newUser);           
            var userLogin = new userForLoginDTO
            {
                Username = "sampleuser",
                Password = "password"
            };
            var responseToken = await Login(userLogin);
            var token = responseToken.Content.ReadAsStringAsync().Result;
            return token;
        }
        
        protected async Task<HttpResponseMessage> CreateUser(userForRegisterDTO userRegistration)
        {
            return await TestClient.PostAsJsonAsync(ApiRoutes.Auth.Register, userRegistration);       
        }

        protected async Task<HttpResponseMessage> TransferMoney(transferMoneyDTO transferMoney)
        {
            return  await TestClient.PostAsJsonAsync(ApiRoutes.User.TransferMoney, transferMoney);
        }

        protected async Task<HttpResponseMessage> Login(userForLoginDTO userLogin)
        {
            return await TestClient.PostAsJsonAsync(ApiRoutes.Auth.Login,userLogin);
        }

        protected async Task<HttpResponseMessage> GetUser(Guid id)
        {
            return await TestClient.GetAsync(ApiRoutes.User.GetUser + "/" + id.ToString());
        }
    }
}
