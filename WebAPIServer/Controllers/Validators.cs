using System.Net.Sockets;

namespace WebAPIServer.Controllers
{
	public static class Validators
	{
		public static AddressFamily ValidateAddressFamily(int? queryFamily)
		{
			AddressFamily family;
			if (!queryFamily.HasValue || (queryFamily != 4 && queryFamily != 6))
			{
				family = AddressFamily.Unknown;
			}
			else family = queryFamily.Value == 4 ? AddressFamily.InterNetwork : AddressFamily.InterNetworkV6;
			return family;
		}

	}
}
