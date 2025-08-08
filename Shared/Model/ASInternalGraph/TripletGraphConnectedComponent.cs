using Shared.Model.ASInternalGraph.GraphElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.ASInternalGraph
{
	public class TripletGraphConnectedComponent
	{
		public ASTripletsGraph Graph { get; private set; }
		public HashSet<ASVertex> ComponentVertexSet { get; private set; }

		public TripletGraphConnectedComponent(ASTripletsGraph graph)
		{
			Graph = graph;
			ComponentVertexSet = new();
		}
	}
}
