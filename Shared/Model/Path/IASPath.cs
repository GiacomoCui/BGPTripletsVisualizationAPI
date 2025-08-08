using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Model.BGP;

namespace Shared.Model.Path
{
    public interface IASPath : IEnumerable<AutonomousSystem>, IEquatable<IASPath>, IReadOnlyList<AutonomousSystem>
	{
		public new AutonomousSystem this[int i] { get; set; }
		public int Length { get; }

		public int GetHashCode();

		public bool AreConsecutive(AutonomousSystem a, AutonomousSystem b)
		{
			int i = 0;
			while(i < Length - 1) 
			{
				if (this[i].Equals(a) && this[i].Equals(b))
				{
					return true;
				}
			}
			return false;
		}
	}
}
