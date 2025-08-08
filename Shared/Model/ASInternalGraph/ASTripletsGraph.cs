using QuikGraph;
using QuikGraph.Algorithms.Search;
using Shared.Model.ASInternalGraph.GraphElements;
using Shared.Model.BGP;
using Shared.Model.Caida;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Shared.Model.ASInternalGraph
{
	public class ASTripletsGraph
	{
		public uint ASNumber { get; set; }
		public AddressFamily Family { get; private set; }
		public BidirectionalGraph<ASVertex, ASEdge> Graph { get; private set; }

		public HashSet<TripletWithData> Triplets { get; private set; }

		public int? ConnectedComponentsNumber { get; private set; } = null;

		public bool IsSingleCP { get; private set; } = false;
		public uint? SingleCP { get; set; }
		private RelationshipCalculator _relationshipCalculator;

		public ASTripletsGraph(IEnumerable<TripletWithData> triplets, CaidaGraph caida, AddressFamily family, uint? singleCP = null)
		{
			if (!triplets.Any()) throw new ArgumentException("Triplets list must not be empty!");
			ASNumber = triplets.First().Current;
			Family = family;
			Triplets = triplets.ToHashSet();
			SingleCP = singleCP;
			IsSingleCP = singleCP != null;
			_relationshipCalculator = new(caida);
			InternalBuildGraph(triplets);
		}


		public HashSet<ASVertex> GetNeighbours(ASVertex v)
		{
			return Graph.InEdges(v).Select(e => e.Source).Concat(Graph.OutEdges(v).Select(e => e.Target)).ToHashSet();
		}
		public HashSet<ASVertex> GetInNeighbous(ASVertex v)
		{
			return Graph.InEdges(v).Select(e => e.Source).ToHashSet();
		}
		public HashSet<ASVertex> GetOutNeighbous(ASVertex v)
		{
			return Graph.OutEdges(v).Select(e => e.Target).ToHashSet();
		}

		private void InternalBuildGraph(IEnumerable<TripletWithData> edges)
		{
			if (!edges.Any()) throw new ArgumentException("Edges cannot be empty!");
			BidirectionalGraph<ASVertex, ASEdge> resultGraph = new();

			foreach (TripletWithData edge in edges)
			{
				ASVertex source = new(edge.Prec);
				ASVertex target = new(edge.Succ);
				int paths = IsSingleCP ? edge.GetPathsNumberSeenBy(SingleCP.Value, Family) : edge.GetTotalPathCount(Family);
				resultGraph.AddVerticesAndEdge(new(source, target, edge, paths)
				{
					PrecRelationship = _relationshipCalculator.ComputeRelationship(edge.Prec, edge.Current),
					SuccRelationship = _relationshipCalculator.ComputeRelationship(edge.Current, edge.Succ)
				});
			}
			Graph = resultGraph;
		}



		public List<TripletGraphConnectedComponent> ComputeConnectedComponents()
		{
			List<TripletGraphConnectedComponent> connectedComponents = new();
			TripletGraphConnectedComponent connectedComponent = null;
			ASVertex firstComponentVertex = new(0);
			ASVertex lastDiscoveredVertex = new(0);

			DepthFirstSearchAlgorithm<ASVertex, ASEdge> depthFirstSearchAlgorithm = new(null, Graph, new Dictionary<ASVertex, GraphColor>(),
				edges =>
				{
					//Aggiungi gli InEdges del nodo, facendo un reverse => trick che vede il grafo come indiretto
					return edges.Concat(Graph.InEdges(lastDiscoveredVertex).Select(x =>
					{
						return new ASEdge(x.Target, x.Source, x.Triplet, x.Tag);
					}));
				});
			depthFirstSearchAlgorithm.DiscoverVertex += (vertex) =>
			{
				if (connectedComponent is null)
				{
					firstComponentVertex = vertex;
					connectedComponent = new(this);
				}
				connectedComponent.ComponentVertexSet.Add(vertex);
				lastDiscoveredVertex = vertex;
			};
			depthFirstSearchAlgorithm.TreeEdge += (edge) =>
			{
				lastDiscoveredVertex = edge.Target;
			};
			depthFirstSearchAlgorithm.FinishVertex += (vertex) =>
			{
				if (vertex == firstComponentVertex)
				{
					connectedComponents.Add(connectedComponent);
					connectedComponent = null;
				}
			};
			depthFirstSearchAlgorithm.Compute();
			ConnectedComponentsNumber = connectedComponents.Count;
			return connectedComponents;
		}


		public bool CheckBipartition(out List<ASVertex> badVertices)
		{
			bool bipartite = true;
			badVertices = new();
			foreach (ASVertex vertex in Graph.Vertices)
			{
				if (Graph.InDegree(vertex) != 0 && Graph.OutDegree(vertex) != 0)
				{
					bipartite = false;
					badVertices.Add(vertex);
				}
			}
			return bipartite;
		}

		public bool ComputeBipartition(out List<ASVertex> InVertices, out List<ASVertex> OutVertices)
		{
			InVertices = new();
			OutVertices = new();
			bool isBipartite = true;
			foreach (ASVertex v in Graph.Vertices)
			{
				if (Graph.OutDegree(v) == 0 && Graph.InDegree(v) != 0)
				{
					InVertices.Add(v);
				}
				else if (Graph.OutDegree(v) != 0 && Graph.InDegree(v) == 0)
				{
					OutVertices.Add(v);
				}
				else if (Graph.InDegree(v) != 0 && Graph.OutDegree(v) != 0)
				{
					isBipartite = false;
				}
			}
			return isBipartite;
		}

	}
}
