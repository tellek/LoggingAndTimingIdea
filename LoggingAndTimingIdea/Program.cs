using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TopherLog;

namespace LoggingAndTimingIdea
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var thread = Thread.CurrentThread.ManagedThreadId;

            int[] jobs = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<string> threads = new List<string>();

            Parallel.ForEach(jobs, (j) =>
            {
                string threadId = Guid.NewGuid().ToString();

                LogIdea.Add(threadId, $"Starting up job {j}.");

                Random rnd = new Random();
                var num = rnd.Next(1, 99);

                using (new LogIdeaTime(threadId, $"{num} Random number process -> "))
                {
                    Thread.Sleep(num);
                }

                using (new LogIdeaTime(threadId, $"1 second sleep process -> "))
                {
                    Thread.Sleep(1000);
                }

                LogIdea.Add(threadId, $"Finishing job.");
                threads.Add(threadId);
            });

            Console.WriteLine("Done.");
            Console.ReadKey();

            foreach (var t in threads)
            {
                Console.WriteLine("=====================================================");
                Console.WriteLine($"Log Count: {LogIdea.Count()}");
                Console.WriteLine("=====================================================");
                Console.WriteLine(LogIdea.Dump(t.ToString()));
                Console.ReadKey();
            }

        }
    }
}
