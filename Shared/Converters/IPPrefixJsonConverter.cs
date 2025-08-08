using MRTSharp.Model.IP;
using Newtonsoft.Json;
using System;

namespace Shared.Converters
{
	public class IPPrefixJsonConverter : JsonConverter
	{
		public override bool CanRead
		{
			get { return false; }
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(IPPrefix);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
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

			if (value is not IPPrefix node)
			{
				return;
			}

			serializer.Serialize(writer, node.ToString());
		}
	}
}
