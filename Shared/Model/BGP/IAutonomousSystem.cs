using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.BGP
{
	internal interface IAutonomousSystem : IEquatable<IAutonomousSystem>
	{
		public uint AsNumber { get; }

		bool IEquatable<IAutonomousSystem>.Equals(IAutonomousSystem other)
		{
			return AsNumber == other.AsNumber;
		}
	}
}
