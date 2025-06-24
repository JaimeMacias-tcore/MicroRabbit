using MicroRabbit.Transfer.Domain.Interfaces;
using MicroRabbit.Transfer.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicroRabbit.Transfer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransferController(ITransferService transferService)
        {
            _transferService = transferService ?? throw new ArgumentNullException(nameof(transferService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransferLog>>> Get()
        {
            var transfers = await _transferService.GetTransfersAsync();

            return Ok(transfers);
        }
    }
}
