using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientTester
{
    class Program
    {
        const int DefaultTaskCount = 32;
        const string Address = "http://<ipAddr>:5000/hello";
        const int numberOfHttpClients = 2;
        static int timeout = 20;//seconds
        static long count = 0;

        static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.UseManagedHttpClientHandler", true);

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < numberOfHttpClients; i++)
            {
                tasks.Add(new Program().RunHttpClientSpammer(timeout));
            }

            Task.WhenAll(tasks).GetAwaiter().GetResult();

            long total = count;
            Console.WriteLine($"Duration: {timeout} seconds, Total: {total}, Rate: {total / timeout}");

            Console.ReadKey();
        }

        public async Task RunHttpClientSpammer(int seconds)
        {
            
            HttpClient client = new HttpClient(new HttpClientHandler())
            {
                BaseAddress = new Uri(Address),
            };

            int taskCount = DefaultTaskCount;

            bool run = true;

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            List<Task> tasks = new List<Task>();
            for (int task = 0; task < taskCount; task++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    while (run)
                    {
                        HttpResponseMessage response = await client.GetAsync("");
                        Interlocked.Increment(ref count);
                        if (stopwatch.Elapsed.Seconds >= seconds)
                        {
                            stopwatch.Stop();
                            break;
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}
