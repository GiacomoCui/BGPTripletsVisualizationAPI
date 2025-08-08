using MessagePack;
using Shared.Model.Path;
using System.Collections;
using System.Collections.Generic;

namespace Shared.Model.Raw
{
	[MessagePackObject]
	public class RawAsPath
	{
		[Key(0)]
		public uint[] AsNumbers;
		public int Length => AsNumbers.Length;

		public RawAsPath(uint[] aSNumbers)
		{
			AsNumbers = aSNumbers;
		}
	}
}
