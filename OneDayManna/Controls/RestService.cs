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
        Random random;

        public RestService()
        {
            random = new Random();

            _client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            });
        }

        public async Task<KoreanManna> GetMannaDataAsync(string uri)
        {
            var mannaData = new KoreanManna();

            try
            {
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    mannaData = JsonConvert.DeserializeObject<KoreanManna>(content);

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

        private string GetApiUrl(string bible, string bookKor, int jang, string jeolRange)
        {
            // bible : kjv
            // bookKor : 창
            // jang : 1
            // jeolRange : 1-10
            var endpoint = "https://api.biblesupersearch.com/";
            return $"{endpoint}api?bible={bible}&reference={bookKor.BibleBookKorToEng()}{jang}:{jeolRange}&data_format=minimal";
        }

        public async Task<EnglishManna> GetEnglishManna(string bookKor, int jang, string jeolRange)
        {
            var url = $"https://bible-api.com/{bookKor.BibleBookKorToEng()}+{jang}:{jeolRange}?translation=kjv";
            try
            {
                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(content);
                    var mannaData = JsonConvert.DeserializeObject<EnglishManna>(content);
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
            return new EnglishManna();
        }

        public async Task<SpanishManna> GetSpanishManna(string bookKor, int jang, string jeolRange)
        {
            var url = GetApiUrl("rv_1909", bookKor, jang, jeolRange);
            try
            {
                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(content);
                    var mannaData = JsonConvert.DeserializeObject<SpanishManna>(content);
                    foreach (var node in mannaData.Results.Content)
                    {
                        Debug.WriteLine(node.Text);
                    }

                    return mannaData;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\tERROR {0}", ex.Message);
            }
            return new SpanishManna();
        }

        public async Task<ChineseManna> GetChineseManna(string bookKor, int jang, string jeolRange)
        {
            var url = GetApiUrl("ckjv_sdt", bookKor, jang, jeolRange);
            try
            {
                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(content);
                    var mannaData = JsonConvert.DeserializeObject<ChineseManna>(content);
                    foreach (var node in mannaData.Results.Content)
                    {
                        Debug.WriteLine(node.Text);
                    }

                    return mannaData;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("\tERROR {0}", ex.Message);
            }
            return new ChineseManna();
        }

        public async Task<Stream> GetRandomImageStream()
        {
            try
            {
                AppManager.PrintStartText("GetRandomImageStream()");

                var imageUrl = Constants.IMAGE_API_ENDPOINT + random.Next(1, 1000);

                var response = await _client.GetAsync(new Uri(imageUrl));
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
