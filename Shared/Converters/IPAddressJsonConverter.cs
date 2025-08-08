using Newtonsoft.Json;
using System;
using System.Net;

namespace Shared.Converters
{
	public class IPAddressJsonConverter : JsonConverter
	{
		public override bool CanRead
		{
			get { return false; }
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(IPAddress);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException("This converter cannot read JSON");
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (serializer is null)
			{
				throw new ArgumentNullException(nameof(serializer));
			}

			if (writer is null)
			{
				throw new ArgumentNullException(nameof(writer));
			}

			if (!(value is IPAddress node))
			{
				return;
			}

			serializer.Serialize(writer, node.ToString());
		}
	}
}
