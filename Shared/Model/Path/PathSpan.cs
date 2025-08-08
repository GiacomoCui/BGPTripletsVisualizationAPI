using Shared.Comparers;
using Shared.Model.BGP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Path
{
    public class PathSpan : IASPath, IEquatable<ASPath>, IEquatable<PathSpan>
	{
		private IASPath _path;
		private int _start;
		private int _end;

		public int Length => _end - _start;
		public int Count => Length;

		private static IndexableEqualityComparer<AutonomousSystem> arrayEqualityComparer = new();

		public PathSpan(IASPath path, int start, int end = -1)
		{
			if (start < 0 || start > path.Length) throw new ArgumentOutOfRangeException(nameof(start));
			if(end != -1 && (end > path.Length || end < start)) throw new ArgumentOutOfRangeException(nameof(end));
			_path = path;
			_start = start;
			if(end == -1) end = path.Length;
			_end = end;
		}

		public PathSpan()
		{
			_path = null;
			_start = 0;
			_end = 0;
		}

		public AutonomousSystem this[int i]
		{
			get {
				if (_start + i >= _end) throw new IndexOutOfRangeException("Index ouf of Span Length");
				return _path[_start + i]; 
			}
			set { 
				if (_start + i >= _end) throw new IndexOutOfRangeException("Index ouf of Span Length"); 
				_path[_start + i] = value; 
			}
		}

		public bool DiscoverPrepending(int prependLen = 1)
		{
			if (_start - prependLen >= 0)
			{
				_start -= prependLen;
				return true;
			}
			return false;
		}
		public bool DiscoverPathEnd(int appendLen = 1)
		{
			if (_end + appendLen <= _path.Length)
			{
				_end += appendLen;
				return true;
			}
			return false;
		}

		public IEnumerator<AutonomousSystem> GetEnumerator()
		{
			return _path.Skip(_start).Take(Length).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _path.Skip(_start).Take(Length).GetEnumerator();
		}

		public bool Equals(ASPath other)
		{
			return arrayEqualityComparer.Equals(this.ToArray(), other.ToArray());
		}
		public override bool Equals(object obj)
		{
			return (obj is ASPath other && Equals(other))
					|| (obj is PathSpan otherSpan && Equals(otherSpan));
		}
		public override int GetHashCode()
		{
			return arrayEqualityComparer.GetHashCode(this.ToArray());
		}

		public bool Equals(PathSpan other)
		{
			return arrayEqualityComparer.Equals(this.ToArray(), other?.ToArray());
		}

		public ASPath ToPath()
		{
			return new ASPath(this);
		}

		public bool Equals(IASPath other)
		{
			return arrayEqualityComparer.Equals(this.ToArray(), other?.ToArray());
		}

		public override string ToString()
		{
			return ToPath().ToString();
		}
	}
}
