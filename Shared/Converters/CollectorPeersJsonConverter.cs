using MRTSharp.Model.IP;
using Newtonsoft.Json;
using Shared.Model.BGP;
using System;

namespace Shared.Converters
{
	public class CollectorPeersJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(CollectorPeer);
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

			if (value is not CollectorPeer node)
			{
				return;
			}

			serializer.Serialize(writer, node.ToString());
		}
	}
}
