using MRTSharp.Model.IP;
using Shared.Comparers;
using Shared.Model.BGP;
using Shared.Model.Caida;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Path
{
    public class ASPath : IASPath, IEquatable<ASPath>, IClonable<ASPath>
	{
		public readonly List<AutonomousSystem> Path;
		private static readonly IndexableEqualityComparer<AutonomousSystem> arrayAsComparer = new();
		public int Length => Path.Count;

		public int Count => Length;

		public ASPath(IEnumerable<AutonomousSystem> path)
		{
			Path = new(path);
		}
		public ASPath(uint[] path, CaidaGraph caida)
		{
			Path = new(path.Length);
			for (int i = 0; i < path.Length; i++)
			{
				Path.Add(caida.GetAsFromUInt(path[i]));
			}
		}
		/// <summary>
		/// Returns a new path instance with the argument AS inserted as first AS of the path
		/// </summary>
		/// <param name="AS"></param>
		/// <returns></returns>
		public ASPath Prepend(AutonomousSystem AS)
		{
			ASPath newPath = Clone();
			newPath.Path.Insert(0, AS);
			return newPath;
		}

		public AutonomousSystem this[int i]
		{
			get { return Path[i]; }
			set { Path[i] = value; }
		}

		public bool Equals(ASPath other)
		{
			return arrayAsComparer.Equals(other?.ToArray(), this.ToArray());
		}
		public bool Equals(IASPath other)
		{
			return arrayAsComparer.Equals(other?.ToArray(), this.ToArray());
		}
		public override bool Equals(object obj)
		{
			return obj is IASPath other && Equals(other);	
		}
		public override int GetHashCode()
		{
			return arrayAsComparer.GetHashCode(Path);
		}
		public override string ToString()
		{
			StringBuilder printable = new();
			foreach (AutonomousSystem asn in Path)
			{
				printable.Append(asn.AsNumber + " ");
			}
			return printable.ToString();
		}

		public IEnumerator<AutonomousSystem> GetEnumerator()
		{
			return Path.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Path.GetEnumerator();
		}

		public ASPath Clone()
		{
			return new(Path);
		}
	}
}
