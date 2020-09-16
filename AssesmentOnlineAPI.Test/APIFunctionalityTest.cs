using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AssesmentOnlineAPI.DTO;
using NUnit.Framework;

namespace AssesmentOnlineAPI.Test
{
    [TestFixture]
    public class APIFunctionalityTest:IntegrationTest
    {

        public APIFunctionalityTest()
        {
            AuthenticateAsync().Wait();
        }
        [Test]
        public async Task Register_CreateUsers_ReturnsNotEmptyGUID()
        {
            var registrationDTO = new userForRegisterDTO()
            {
                Username = "NotEmptyGui",
                FirstName = "NotEmptyGui",
                LastName = "NotEmptyGui",
                Password = "NotEmptyGui",
                InitialBalance = 10
            };
            var responseMessage = await CreateUser(registrationDTO);
            var resultID = await responseMessage.Content.ReadAsAsync<Guid>();
            var isValid = resultID == Guid.Empty ? false : true;

            Assert.True(isValid);
        }
        [Test]
        public async Task Register_RegisterWithExistingUser_ReturnsBadRequest()
        {
            var registrationDTO = new userForRegisterDTO()
            {
                Username = "sampleuser",
                FirstName = "sample",
                LastName = "sample",
                Password = "password",
                InitialBalance = 10
            };
            var response = await CreateUser(registrationDTO);
            var responseMessage = response.Content.ReadAsStringAsync().Result;
            Assert.That(HttpStatusCode.BadRequest, Is.EqualTo(response.StatusCode));
            Assert.That(responseMessage, Does.StartWith("Username").IgnoreCase);
            Assert.That(responseMessage, Does.EndWith("already exists").IgnoreCase);
        }
        [Test]
        public async Task Login_LoginWithNonExistingUser_ReturnsUnauthorize()
        {
            var userLogin = new userForLoginDTO()
            {
                Username = "NonExisting",
                Password = "NonExisting"
            };
            var response = await Login(userLogin);
            Assert.That(HttpStatusCode.Unauthorized, Is.EqualTo(response.StatusCode));
        }
        [Test]
        public async Task User_AccessUserWithoutAuthentication_ReturnsUnauthorized()
        {
            InvalidaAuthentication();
            var registrationDTO = new userForRegisterDTO()
            {
                Username = "UnauthorizeUser",
                FirstName = "sample",
                LastName = "sample",
                Password = "password",
                InitialBalance = 10
            };
            var responseMessage = await CreateUser(registrationDTO);
            var resultID = await responseMessage.Content.ReadAsAsync<Guid>();
            var response = await GetUser(resultID);
            
            Assert.That(HttpStatusCode.Unauthorized, Is.EqualTo(response.StatusCode));
        }
        [Test]
        public async Task TransferMoney_TransferGreaterAmountThanYourBalance_ReturnsCannotTransferGreaterThanYourAmount()
        {
            //Arrange
            var expectedResult = "You cannot transfer money greater than your balance";
            var sourceUser = new userForRegisterDTO()
            {
                Username = "sourceuser1",
                FirstName = "sourceuser1",
                LastName = "sourceuser1",
                Password = "password1",
                InitialBalance = 10
            };
            var destinationUser = new userForRegisterDTO()
            {
                Username = "destinationuser1",
                FirstName = "destinationuser1",
                LastName = "destinationuser1",
                Password = "password1",
                InitialBalance = 0
            };
            var sourceUserCreated = await CreateUser(sourceUser);
            var sourceUserID = await sourceUserCreated.Content.ReadAsAsync<Guid>();
            var destinationUserCreated = await CreateUser(destinationUser);
            var destinationUserID = await destinationUserCreated.Content.ReadAsAsync<Guid>();
            //await AuthenticateAsync();


            //Act
            var transferMoney = new transferMoneyDTO()
            {
                SourceAccountID=sourceUserID,
                DestinationAccountID=destinationUserID,
                Amount=11
            };       
            var response = await TransferMoney(transferMoney);
            string responseMessage = response.Content.ReadAsStringAsync().Result;

            // Assert                 
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(expectedResult, Is.EqualTo(responseMessage));

        }
        [Test]
        public async Task TransferMoney_TransferToSameSourceAccountID_ReturnsCannotTransferToSameAccount()
        {
            //Arrange
            var expectedResult = "You cannot transfer money to same account";
            var sourceUser = new userForRegisterDTO()
            {
                Username = "sourceuser2",
                FirstName = "sourceuser2",
                LastName = "sourceuser2",
                Password = "password2",
                InitialBalance = 10
            };
         
            var sourceUserCreated = await CreateUser(sourceUser);            
            var sourceUserID = await sourceUserCreated.Content.ReadAsAsync<Guid>();
            //await AuthenticateAsync();


            //Act
            var transferMoney = new transferMoneyDTO()
            {
                SourceAccountID = sourceUserID,
                DestinationAccountID = sourceUserID,
                Amount = 10
            };
            var result = await TransferMoney(transferMoney);
            string responseMessage = result.Content.ReadAsStringAsync().Result;

            // Assert
            //result.Should().BeOfType<BadRequestResult>();
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(expectedResult, Is.EqualTo(responseMessage));

        }
        [Test]
        public async Task TransferMoney_TransferConcurrentData_ReturnsBadRequest()
        {
            var isWithBadRequest = false;
            List<HttpResponseMessage> responses =new List<HttpResponseMessage>();
            var sourceUser = new userForRegisterDTO()
            {
                Username = "sourceuser",
                FirstName = "sourceuser",
                LastName = "sourceuser",
                Password = "password",
                InitialBalance = 10
            };
            var destinationUser = new userForRegisterDTO()
            {
                Username = "destinationuser",
                FirstName = "destinationuser",
                LastName = "destinationuser",
                Password = "password",
                InitialBalance = 0
            };
            var sourceUserCreated = await CreateUser(sourceUser);
            var sourceUserID = await sourceUserCreated.Content.ReadAsAsync<Guid>();
            var destinationUserCreated = await CreateUser(destinationUser);
            var destinationUserID = await destinationUserCreated.Content.ReadAsAsync<Guid>();           
            var transferMoney = new transferMoneyDTO()
            {
                SourceAccountID = sourceUserID,
                DestinationAccountID = destinationUserID,
                Amount = 1
            };
            List<Task> tasks = new List<Task>();
            for (int i = 1; i <= 11; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    var response = TransferMoney(transferMoney);
                    responses.Add(response.Result);
                }));               
            }
            Task.WaitAll(tasks.ToArray());
            foreach(HttpResponseMessage httpResponse in responses)
            {
                if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                    isWithBadRequest = true;
            }
       
            Assert.That(isWithBadRequest, Is.EqualTo(true));
        }
    
    }
}
