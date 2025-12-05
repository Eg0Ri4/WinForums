using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SmartTravelPLanner.Core;

namespace SmartTravelPLanner.Core
{
    public class Traveler : ICloneable
    {
        private string name;
        private string? currentLocation;
        private readonly List<string> route = new List<string>();

        public Traveler(string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string GetName()
        {
            return name;
        }

        public void SetLocation(string? location)
        {
            if (string.IsNullOrEmpty(location))
            {
                currentLocation = location;
                return;
            }

            char firstChar = location[0];
            if (firstChar >= 'a' && firstChar <= 'z')
            {
                firstChar = (char)(firstChar - 32);
            }
            currentLocation = firstChar + location.Substring(1);
        }

        public string? GetLocation()
        {
            return currentLocation;
        }

        public List<string> GetRouteItems()
        {
            return new List<string>(route);
        }

        public void AddCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                throw new ArgumentException("City name cannot be empty.");
            }
            route.Add(city);
        }

        public string GetRoute()
        {
            return string.Join(" -> ", route);
        }

        public override string ToString()
        {
            return $"Traveler: {name} | Location: {currentLocation} | Route: {GetRoute()}";
        }

        public void ClearRoute()
        {
            route.Clear();
        }

        public int GetStopCount()
        {
            return route.Count;
        }

        public bool HasCity(string city)
        {
            return route.Contains(city);
        }

        public void SortRoute()
        {
            route.Sort();
        }

        public bool RemoveCity(string city)
        {
            if (route.Contains(city))
            {
                route.Remove(city);
                return true;
            }
            return false;
        }

        public string? GetNextStop()
        {
            if (route.Count > 0)
            {
                return route[0];
            }
            return null;
        }

        public string this[int index] => route[index];

        public static bool operator ==(Traveler? a, Traveler? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.name == b.name && a.currentLocation == b.currentLocation;
        }

        public static bool operator !=(Traveler? a, Traveler? b) => !(a == b);

        public override bool Equals(object? obj)
        {
            return obj is Traveler other && this == other;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, currentLocation);
        }

        public object Clone()
        {
            Traveler copy = new Traveler(this.name);
            copy.SetLocation(this.currentLocation);
            for (int i = 0; i < this.route.Count; i++)
            {
                copy.AddCity(this.route[i]);
            }
            return copy;
        }

        public void SaveToFile(string filePath)
        {
            string nameJson = JsonSerializer.Serialize(this.name);
            string locJson = JsonSerializer.Serialize(this.currentLocation);
            string routeJson = JsonSerializer.Serialize(this.route);

            routeJson = routeJson.Replace("\",\"", "\", \"");

            string json = "{\n" +
                          $"\"name\": {nameJson},\n" +
                          $"\"currentLocation\": {locJson},\n" +
                          $"\"route\": {routeJson}\n" +
                          "}";

            File.WriteAllText(filePath, json);
        }

        public static Traveler LoadFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException();

                string json = File.ReadAllText(filePath);
                var travelerData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

                string? name = travelerData["name"].GetString();
                string? currentLocation = travelerData["currentLocation"].GetString();

                List<string> route = new List<string>();
                if (travelerData.ContainsKey("route"))
                {
                    foreach (var city in travelerData["route"].EnumerateArray())
                        route.Add(city.GetString()!);
                }

                Traveler traveler = new Traveler(name ?? string.Empty);
                traveler.SetLocation(currentLocation);
                route.ForEach(city => traveler.AddCity(city));

                return traveler;
            }
            catch (FileNotFoundException)
            {
                throw new FileLoadException("File doesn't exist");
            }
            catch (JsonException)
            {
                throw new FileLoadException("Invalid travel data");
            }
            catch (KeyNotFoundException)
            {
                throw new FileLoadException("Invalid travel data");
            }
        }

        public bool PlanRouteTo(string destination, CityGraph map)
        {
            string? start;

            if (!string.IsNullOrEmpty(currentLocation))
                start = currentLocation;
            else if (route.Count > 0)
                start = route[0];
            else
            {
                return false;
            }

            List<string> shortestPath = map.FindShortestPath(start!, destination);

            if (shortestPath.Count == 0)
                return false;

            route.Clear();
            route.AddRange(shortestPath);

            return true;
        }
    }
}


