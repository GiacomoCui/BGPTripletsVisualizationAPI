using Microsoft.AspNetCore.Mvc;
using Shared.Database.Services;
using Shared.Model.BGP;
using System.Net;
using System.Net.Sockets;
using static WebAPIServer.Controllers.Validators;

namespace WebAPIServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[ResponseCache(Duration = 31536000, VaryByQueryKeys = new[] { "*" })]
	public class TripletsController : ControllerBase
	{

		private readonly ILogger<TripletsController> _logger;
		private readonly TripletService _service;

		public TripletsController(ILogger<TripletsController> logger, TripletService service)
		{
			_logger = logger;
			_service = service;
		}

		[HttpGet]
		public IActionResult Get()
		{
			return Ok();
		}

		[HttpGet("tiplet/{id:length(24)}")]
		public IActionResult GetById(string id)
		{
			return Ok(_service.GetByID(id));
		}

		[HttpGet("middleASes")]
		public IActionResult GetAllMiddleASes()
		{
			return Ok(_service.GetAllMiddleASes());
		}

		[HttpGet("findall")]
		public IActionResult GetByMiddleAS([FromQuery] uint middleAS, [FromQuery] int? queryFamily)
		{
			AddressFamily family;
			if (!queryFamily.HasValue || (queryFamily != 4 && queryFamily != 6))
			{
				family = AddressFamily.Unknown;
			}
			else family = queryFamily.Value == 4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6;

			return Ok(_service.GetAllTripletsOfAs(middleAS, family));
		}

		[HttpGet("availableCPs")]
		public IActionResult GetAvailableCPs([FromQuery] uint middleAS)
		{
			return Ok(_service.GetAllTripletsCPs(middleAS));
		}

		[HttpGet("findOnecpFull")]
		public IActionResult GetByMiddleAS([FromQuery] uint middleAS, [FromQuery] string peerIPAddress, [FromQuery] uint peerAS, [FromQuery] int? queryFamily)
		{
			AddressFamily family = ValidateAddressFamily(queryFamily);

			IPAddress cpAddr;
			try
			{
				cpAddr = IPAddress.Parse(peerIPAddress);
			}
			catch { return BadRequest($"CP Address {peerIPAddress} not valid");  }

			CollectorPeer peer = new(cpAddr, peerAS);

			return Ok(_service.GetAllTripletsOfASWithCP(middleAS, peer, family));
		}

        [HttpGet("findOnecp")]
        public IActionResult GetByMiddleASShortData([FromQuery] uint middleAS, [FromQuery] string peerIPAddress, [FromQuery] uint peerAS, [FromQuery] int? queryFamily)
        {
            AddressFamily family = ValidateAddressFamily(queryFamily);

            IPAddress cpAddr;
            try
            {
                cpAddr = IPAddress.Parse(peerIPAddress);
            }
            catch { return BadRequest($"CP Address {peerIPAddress} not valid"); }
            CollectorPeer peer = new(cpAddr, peerAS);

            return Ok(_service.GetAllTripletsOfASWithCPSimpleData(middleAS, peer, family));
        }
    }
}
