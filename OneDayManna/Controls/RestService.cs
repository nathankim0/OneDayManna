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
        private static readonly string bibleApiEndpoint = "https://bible-api.com/";

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

        public async Task<JsonMannaOtherLanguageModel> GetMultilanguageManna(string bookKor, int jang, string jeolRange)
        {
            var url = $"{bibleApiEndpoint}{bookKor.BibleBookKorToEng()}+{jang}:{jeolRange}?translation=kjv";
            try
            {
                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(content);
                    var mannaData = JsonConvert.DeserializeObject<JsonMannaOtherLanguageModel>(content);
                    foreach(var node in mannaData.Verses)
                    {
                        try
                        {
                            node.Text = node.Text.TrimStart().TrimEnd().Replace("\n", "");
                            Debug.WriteLine(node.Text);
                        }
                        catch(Exception e) { AppManager.PrintException("GetMultilanguageManna() trim", e.Message); }
                    }

                    return mannaData;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\tERROR {0}", ex.Message);
            }
            return new JsonMannaOtherLanguageModel();
        }
        public async Task<Stream> GetRandomImageStream()
        {
            try
            {
                AppManager.PrintStartText("GetRandomImageStream()");

                var random = new Random();

                var response = await _client.GetAsync(new Uri(Constants.IMAGE_API_ENDPOINT + random.Next()));
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    AppManager.PrintCompleteText("GetRandomImageStream()");
                    return stream;
                }
                else
                {
                    throw new HttpRequestException();
                }
            }
            catch (Exception ex)
            {
                AppManager.PrintException("SetImageSource()", ex.Message);
                return Stream.Null;
            }
        }
    }
}
