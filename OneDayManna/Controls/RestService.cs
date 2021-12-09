using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OneDayManna
{
    public class RestService
    {
        private static RestService _instance;
        public static RestService Instance => _instance ?? (_instance = new RestService());

        private readonly HttpClient _client;

        public RestService()
        {
            _client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            });
        }

        public async Task<JsonMannaModel> GetMannaDataAsync(string uri)
        {
            var mannaData = new JsonMannaModel();

            try
            {
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    mannaData = JsonConvert.DeserializeObject<JsonMannaModel>(content);

                    Console.WriteLine($"@@@@@{mannaData.Verse}");
                    foreach(var node in mannaData.Contents)
                    {
                        Console.WriteLine($"@@@@@{node}");
                    }
                }
            }
            catch (Exception ex)
            {                
                Debug.WriteLine("\tERROR {0}", ex.Message);
            }

            return mannaData;
        }

        public async Task<Stream> GetRandomImageStream()
        {
            try
            {
                var random = new Random();

                var response = await _client.GetAsync(new Uri(Constants.IMAGE_API_ENDPOINT + random.Next()));
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStreamAsync();
                }
                return Stream.Null;
            }
            catch (Exception ex)
            {
                AppManager.ExceptionCommonText("SetImageSource()", ex.Message);
                return Stream.Null;
            }
        }
    }
}
