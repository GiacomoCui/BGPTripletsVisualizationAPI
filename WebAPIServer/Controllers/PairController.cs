using Microsoft.AspNetCore.Mvc;
using Shared.Database.Services;
using Shared.Model;
using Shared.Model.BGP;
using System.Net;
using System.Net.Sockets;
using static WebAPIServer.Controllers.Validators;

namespace WebAPIServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[ResponseCache(Duration = 31536000, VaryByQueryKeys = new[] { "*" })]
	public class PairController : ControllerBase
	{
		private readonly PairServices _service;

		public PairController(PairServices service) 
		{
			_service = service;
		}

		[HttpGet("prefixes")]
		public IActionResult GetSeenPrefixes([FromQuery] uint prec,
												[FromQuery] uint middleAS,
												[FromQuery] uint succ,
												[FromQuery] string peerIPAddress,
												[FromQuery] uint peerAS,
												[FromQuery] int? queryFamily)
		{
			AddressFamily family = ValidateAddressFamily(queryFamily);

			IPAddress cpAddr;
			try
			{
				cpAddr = IPAddress.Parse(peerIPAddress);
			}
			catch { return BadRequest($"CP Address {peerIPAddress} not valid"); }

			CollectorPeer peer = new(cpAddr, peerAS);
			Triplet t = new(prec, middleAS, succ); 
			return Ok(_service.GetSeenIPsForPeer(t, peer, family));
		}
	}
}
