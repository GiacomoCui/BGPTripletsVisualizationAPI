using MessagePack;
using MessagePack.Formatters;
using System.Buffers;
using System.Net;

namespace Shared.Converters
{
	public class IPFormatter : IMessagePackFormatter<IPAddress>
	{
		public void Serialize(ref MessagePackWriter writer, IPAddress value, MessagePackSerializerOptions options)
		{
			if (value == null)
			{
				writer.WriteNil();
				return;
			}

			writer.WriteString(value.GetAddressBytes());
		}

		public IPAddress Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
		{
			if (reader.TryReadNil())
			{
				return null;
			}

			options.Security.DepthStep(ref reader);

			ReadOnlySequence<byte> address = reader.ReadBytes().Value;

			reader.Depth--;

			return new IPAddress(address.ToArray());
		}
	}
}
