using System;
using System.Collections.Generic;
using SensorSim.Util;

namespace HomeDesigner
{
    public class Graph<T> {
        public Graph() {}
        public Graph(IEnumerable<T> vertices, IEnumerable<Pair<T,T>> edges) {
            foreach(var vertex in vertices)
                AddVertex(vertex);

            foreach(var edge in edges)
                AddEdge(edge);
        }

        public Dictionary<T, HashSet<T>> AdjacencyList { get; } = new Dictionary<T, HashSet<T>>();

        public void AddVertex(T vertex) {
            AdjacencyList[vertex] = new HashSet<T>();
        }

        public void AddEdge(Pair<T,T> edge) {
            if (AdjacencyList.ContainsKey(edge.First) && AdjacencyList.ContainsKey(edge.Second)) {
                AdjacencyList[edge.First].Add(edge.Second);
                AdjacencyList[edge.Second].Add(edge.First);
            }
        }
    }
}