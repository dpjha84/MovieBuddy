using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieBuffLib
{
    public class YoutubeProvider
    {
        public YoutubeProvider()
        {
            //var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            //{
            //    ApiKey = "AIzaSyDW6GsPkAqVmTiKxTp6fwCFwehpICYbgB0",
            //    ApplicationName = this.GetType().ToString()
            //});
            //searchListRequest = youtubeService.Search.List("snippet");
            //searchListRequest.MaxResults = 1;
        }

        public async Task<string> GetTrailer(string name)
        {
            string channelUrl = $"https://www.googleapis.com/youtube/v3/search?maxResults=1&part=snippet&q={name}%20official%20trailer&key=AIzaSyDW6GsPkAqVmTiKxTp6fwCFwehpICYbgB0";
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = httpClient.GetStringAsync(channelUrl);
                    var response = JsonConvert.DeserializeObject<YouTubeApiListRoot>(json.Result);
                    var trailer = response.Items[0].Id.VideoId;
                    return trailer;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}