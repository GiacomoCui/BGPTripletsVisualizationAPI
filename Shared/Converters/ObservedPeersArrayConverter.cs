using Shared.Model.BGP;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace Shared.Converters;

public class ObservedPeersArrayConverter : JsonConverter<Dictionary<CollectorPeer, int>>
{
	public override void WriteJson(JsonWriter writer, Dictionary<CollectorPeer, int> value, JsonSerializer serializer)
	{
		writer.WriteStartArray();

		foreach (var kvp in value)
		{
			writer.WriteStartObject();
			writer.WritePropertyName("peerIPAddress");
			writer.WriteValue(kvp.Key.PeerIPAddress.ToString());
			writer.WritePropertyName("peerAS");
			writer.WriteValue(kvp.Key.PeerAS);
			writer.WritePropertyName("count");
			writer.WriteValue(kvp.Value);
			writer.WriteEndObject();
		}

		writer.WriteEndArray();
	}

	public override Dictionary<CollectorPeer, int> ReadJson(JsonReader reader, Type objectType, Dictionary<CollectorPeer, int> existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		var result = new Dictionary<CollectorPeer, int>();

		var array = JArray.Load(reader);
		foreach (var item in array)
		{
			var ipString = item["peerIPAddress"]?.ToString();
			var peerAS = item["peerAS"]?.ToObject<uint>() ?? 0;
			var count = item["count"]?.ToObject<int>() ?? 0;

			if (IPAddress.TryParse(ipString, out var ip))
			{
				var peer = new CollectorPeer(ip, peerAS);
				result[peer] = count;
			}
		}

		return result;
	}

	public override bool CanRead => true;
}