using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Numerics;

namespace Shared.Converters
{

	public class BigIntegerMongodbConverter : SerializerBase<BigInteger>
	{
		public override BigInteger Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			if (context is null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			string val = context.Reader.ReadString();
			return BigInteger.Parse(val);
		}

		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, BigInteger value)
		{
			if (context is null)
			{
				throw new ArgumentNullException(nameof(context));
			}
			context.Writer.WriteString(value.ToString());
		}
	}
}
