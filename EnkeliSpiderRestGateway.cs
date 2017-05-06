using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RestClient
{
    public class EnkeliSpiderGateway
    {
        private const string SECURITY_TOKEN_HEADER = "es-token";
        private const string START_EVENT_ENDPOINT = "api/v2/event/start";
        private const string STOP_EVENT_ENDPOINT = "api/v2/event/stop";
        private const string GET_RESULTS_ENDPOINT = "api/v2/event/{0}/results";
        private const string DOWNLOAD_RESULT_ENDPOINT = "api/v2/event/{0}/result?name={1}";
        private const string RESULTS_DIRECTORY = @"C:\test\";

        private HttpClient client = new HttpClient();

        public EnkeliSpiderGateway(string address, string token)
        {
            client.BaseAddress = new Uri(address);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add(SECURITY_TOKEN_HEADER, token);
        }

        public async Task<Event> StartEventAsync()
        {
            var response = await client.PostAsync(START_EVENT_ENDPOINT, null);
            response.EnsureSuccessStatusCode();
            var newEvent = await response.Content.ReadAsAsync<Event>();
            return newEvent;
        }

        public async Task<Event> StopEventAsync()
        {
            var response = await client.PostAsync(STOP_EVENT_ENDPOINT, null);
            response.EnsureSuccessStatusCode();
            var newEvent = await response.Content.ReadAsAsync<Event>();
            return newEvent;
        }

        public async Task<List<string>> RetrieveResultsAsync(int eventId)
        {
            var response = await client.GetAsync(string.Format(GET_RESULTS_ENDPOINT, eventId));
            response.EnsureSuccessStatusCode();
            var results = await response.Content.ReadAsAsync<List<string>>();
            return results;
        }

        public async Task<bool> DownloadResultAsync(int eventId, string name)
        {
            var response = await client.GetAsync(string.Format(DOWNLOAD_RESULT_ENDPOINT, eventId, name));
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStreamAsync();
            using (var fileStream = File.Create(RESULTS_DIRECTORY + name))
            {
                result.Seek(0, SeekOrigin.Begin);
                result.CopyTo(fileStream);
            }
            return true;
        }

        public class Event
        {
            public int Id { get; set; }
            public string Status { get; set; }

            public string GetInfo()
            {
                return $"Event {Id}: {Status}";
            }
        }
    }
}
