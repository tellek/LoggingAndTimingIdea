using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace TopherLog
{
    public static class LogIdea
    {
        public static ConcurrentDictionary<string, List<(long, string)>> Logs = new ConcurrentDictionary<string, List<(long, string)>>();
        public static int MaxLogRetensionMinutes = 5; 

        public static void Add(string id, string logLine)
        {
            List<(long, string)> oldItem;
            List<(long, string)> newItem;
            if (Logs.TryGetValue(id, out oldItem))
            {
                newItem = oldItem.GetRange(0, oldItem.Count);
                newItem.Add((oldItem.Count + 1, logLine));
                Logs.TryUpdate(id, newItem, oldItem);
            }
            else
            {
                newItem = new List<(long, string)>
                {
                    (1, logLine)
                };
                Logs.TryAdd(id, newItem);
            }
        }

        public static string Dump(string id)
        {
            List<(long, string)> oldItem;
            List<(long, string)> newItem = new List<(long, string)>();
            Logs.TryGetValue(id, out oldItem);

            oldItem = oldItem.OrderBy(o => o.Item1).ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var item in oldItem)
            {
                sb.AppendLine(item.Item2);
            }

            Destroy(id);

            return sb.ToString();
        }

        public static bool Destroy(string id)
        {
            List<(long, string)> removedItem;
            return Logs.TryRemove(id, out removedItem);
        }

        public static int Count()
        {
            return Logs.Count();
        }

        public static string GetThreadId()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static void CleanLogs()
        {
            
        }
    }

    public class LogIdeaTime : IDisposable
    {
        private Stopwatch _timer;
        private string _logLine;
        private string _id;

        public LogIdeaTime(string id, string logLine)
        {
            _logLine = logLine;
            _id = id;
            _timer = new Stopwatch();
            _timer.Start();
        }

        public LogIdeaTime(string logLine)
        {
            _logLine = logLine;
            _id = Thread.CurrentThread.ManagedThreadId.ToString();
            _timer = new Stopwatch();
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Stop();
            string logLine = $"[Elapsed: {_timer.ElapsedMilliseconds} ms] {_logLine}";

            LogIdea.Add(_id, logLine);
        }
    }
}

