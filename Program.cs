using System;
using System.Threading.Tasks;

namespace RestClient
{
    class Program
    {
        private const string ADDRESS = "http://localhost:9090/";
        private const string SECURITY_TOKEN = "in3a0497rg2ncak1ml6ttc6d2n";

        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            Console.WriteLine("Press enter to start an event");
            Console.ReadLine();
            try
            {
                var client = new EnkeliSpiderGateway(ADDRESS, SECURITY_TOKEN);
                var currentEvent = await client.StartEventAsync();
                Console.WriteLine(currentEvent.GetInfo());
                Console.WriteLine("Press enter to finish the event");
                Console.ReadLine();
                currentEvent = await client.StopEventAsync();
                Console.WriteLine(currentEvent.GetInfo());
                Console.WriteLine($"Results of event {currentEvent.Id}:");
                var results = await client.RetrieveResultsAsync(currentEvent.Id);
                results.ForEach(Console.WriteLine);
                foreach (var result in results)
                {
                    await client.DownloadResultAsync(currentEvent.Id, result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
