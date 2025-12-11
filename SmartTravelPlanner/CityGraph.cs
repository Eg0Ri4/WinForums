using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace SmartTravelPlanner.Core
{
    public class CityGraph
    {
        private readonly Dictionary<string, List<(string neighbor, int distance)>> adjacencyList = new();

        private CityGraph() { }

        public static CityGraph LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Map file not found.");

            var graph = new CityGraph();
            string[] lines = File.ReadAllLines(filePath);
            int lineNumber = 0;

            foreach (string rawLine in lines)
            {
                lineNumber++;
                string line = rawLine?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split(',');
                if (parts.Length != 2)
                    throw new FormatException($"Invalid format on line {lineNumber}: expected 'CityA-CityB,Distance'");

                string citiesPart = parts[0].Trim();
                string distancePart = parts[1].Trim();

                if (!int.TryParse(distancePart, out int distance) || distance <= 0)
                    throw new FormatException($"Invalid distance on line {lineNumber}: '{distancePart}' is not a positive number");

                var cities = citiesPart.Split('-');
                if (cities.Length != 2)
                    throw new FormatException($"Invalid cities format on line {lineNumber}: expected 'CityA-CityB'");

                string cityA = cities[0].Trim();
                string cityB = cities[1].Trim();

                if (string.IsNullOrWhiteSpace(cityA) || string.IsNullOrWhiteSpace(cityB))
                    throw new FormatException($"Empty city name on line {lineNumber}");

                if (cityA.Equals(cityB, StringComparison.OrdinalIgnoreCase))
                    throw new FormatException($"Self-loop detected on line {lineNumber}: {cityA} to {cityA}");

                graph.AddEdge(cityA, cityB, distance);
                graph.AddEdge(cityB, cityA, distance); 
            }

            if (graph.adjacencyList.Count == 0)
                throw new FormatException("Map file is empty or contains no valid connections.");

            return graph;
        }

        private void AddEdge(string from, string to, int distance)
        {
            if (!adjacencyList.ContainsKey(from))
                adjacencyList[from] = new List<(string, int)>();

            adjacencyList[from].Add((to, distance));
        }

        public bool ContainsCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city)) return false;

            return adjacencyList.ContainsKey(city.Trim());
        }

        public List<string> FindShortestPath(string from, string to)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                return new List<string>();

            if (!ContainsCity(from) || !ContainsCity(to))
                return new List<string>();

            var distances = new Dictionary<string, int>();
            var previous = new Dictionary<string, string>();
            var unvisited = new PriorityQueue<string, int>();

            foreach (var city in adjacencyList.Keys)
            {
                distances[city] = int.MaxValue;
            }

            distances[from] = 0;
            unvisited.Enqueue(from, 0);

            while (unvisited.Count > 0)
            {
                string current = unvisited.Dequeue();

                if (current == to) 
                    break;

                if (!adjacencyList.ContainsKey(current)) 
                    continue;

                foreach (var (neighbor, dist) in adjacencyList[current])
                {
                    int newDist = distances[current] + dist;

                    if (newDist < distances.GetValueOrDefault(neighbor, int.MaxValue))
                    {
                        distances[neighbor] = newDist;
                        previous[neighbor] = current;
                        unvisited.Enqueue(neighbor, newDist);
                    }
                }
            }

            var path = new List<string>();
            string ? step = to;

            while (step != null)
            {
                path.Insert(0, step);
                if (!previous.TryGetValue(step, out step)) break;
            }

            return path.Count > 0 && path[0] == from ? path : new List<string>();
        }

        public int GetPathDistance(List<string> path)
        {
            if (path == null || path.Count < 2) return 0;

            int total = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (!adjacencyList.TryGetValue(path[i], out var neighbors))
                    return -1;

                var edge = neighbors.Find(n => n.neighbor == path[i + 1]);
                if (edge == default) return -1;

                total += edge.distance;
            }
            return total;
        }
    }

    internal class PriorityQueue<T>
    {
        private readonly List<(T item, int priority)> elements = new();

        public int Count => elements.Count;

        public void Enqueue(T item, int priority)
        {
            elements.Add((item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;
            int bestPriority = elements[0].priority;

            for (int i = 1; i < elements.Count; i++)
            {
                if (elements[i].priority < bestPriority)
                {
                    bestPriority = elements[i].priority;
                    bestIndex = i;
                }
            }

            T result = elements[bestIndex].item;
            elements.RemoveAt(bestIndex);
            return result;
        }
    }
}