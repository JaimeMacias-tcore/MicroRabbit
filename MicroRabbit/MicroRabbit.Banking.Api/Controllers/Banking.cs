using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicroRabbit.Banking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Banking : ControllerBase
    {
        private readonly IAccountService _accountService;
        public Banking(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }


        [HttpGet("accounts")]
        public ActionResult<IEnumerable<Account>> GetAccounts()
        {
            var accounts = _accountService.GetAccounts();
            return Ok(accounts);
        }
    }
}
