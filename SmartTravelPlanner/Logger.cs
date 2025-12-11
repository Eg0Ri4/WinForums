using System;
using System.Collections.Generic;
using System.IO;

namespace SmartTravelPlanner.Core
{
    public class Logger<T>
    {
        private readonly List<T> entries = new List<T>();

        public void Add(T entry)
        {
            entries.Add(entry);
        }

        public void Flush(string filePath)
        {
            using (var writer = new StreamWriter(filePath, append: true))
            {
                foreach (var entry in entries)
                {
                    writer.WriteLine(entry?.ToString());
                }
            }
            entries.Clear();
        }
    }
}
