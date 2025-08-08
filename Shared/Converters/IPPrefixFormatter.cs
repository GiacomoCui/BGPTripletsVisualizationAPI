using MessagePack;
using MessagePack.Formatters;
using MRTSharp.Model.IP;
using System;
using System.Buffers;
using System.Net.Sockets;

namespace Shared.Converters
{
	public class IPPrefixFormatter : IMessagePackFormatter<IPPrefix>
	{
		public void Serialize(ref MessagePackWriter writer, IPPrefix value, MessagePackSerializerOptions options)
		{
			if (value == null)
			{
				writer.WriteNil();
				return;
			}

			writer.WriteArrayHeader(3);
			writer.WriteInt32((int)value.Family);
			writer.WriteString(value.Prefix);
			writer.WriteInt32(value.Cidr);
		}

		public IPPrefix Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
		{
			if (reader.TryReadNil())
			{
				return null;
			}

			options.Security.DepthStep(ref reader);

			int count = reader.ReadArrayHeader();
			if (count != 3)
				throw new Exception("Unable to parse IPPrefix");

			AddressFamily family = (AddressFamily)reader.ReadInt32();
			ReadOnlySequence<byte> prefix = reader.ReadBytes().Value;
			int cidr = reader.ReadInt32();

			reader.Depth--;

			return new IPPrefix(prefix.ToArray(), cidr, family);
		}
	}
}
