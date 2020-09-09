using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssesmentOnlineAPI.DTO;
using AssesmentOnlineAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AssesmentOnlineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _services;
        public UserController(IUserServices services)
        {
            _services = services;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = _services.GetUser(id);
            var _userDTO = new userDTO();
            _userDTO = await user;
            return Ok(_userDTO);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferMoney(transferMoneyDTO transfer)
        {
            if (transfer.SourceAccountID == transfer.DestinationAccountID)
                return BadRequest("You cannot transfer money to same account");
            if(!await _services.isAvailableToTransfer(transfer.SourceAccountID, transfer.Amount))
                return BadRequest("You cannot transfer money greater than your balance");

            var newTransaction=await _services.TransferMoney(transfer.SourceAccountID, transfer.DestinationAccountID, transfer.Amount);
            if (newTransaction == null)
                return BadRequest();


            return Ok(newTransaction.Id);
        }
    }
}