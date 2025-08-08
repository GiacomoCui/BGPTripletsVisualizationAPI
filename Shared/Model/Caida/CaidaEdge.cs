using QuikGraph;
using Shared.Model.BGP;
using static Shared.Enums;

namespace Shared.Model.Caida
{
    public class CaidaEdge : TaggedEdge<AutonomousSystem, ASRelationship>, IEdge<AutonomousSystem>
	{

		public bool Violated;

		public CaidaEdge(AutonomousSystem source, AutonomousSystem target, ASRelationship tag, bool violated = false)
			: base(source, target, tag)
		{
			Violated = violated;
		}
	}
}
