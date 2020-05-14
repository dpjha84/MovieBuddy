using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MovieBuffLib
{
    public class ToiProvider : ReviewProvider
    {
        public override string GetUrl(string name)
        {
            var movieName1 = name.Replace(" ", "+");
            var movieName2 = name.Replace(" ", "-");
            var url1 = $"https://www.google.co.in/search?q={movieName1}+review";
            string ss = $"https://timesofindia.indiatimes.com/entertainment/hindi/movie-reviews/{movieName2}/movie-review/";
            using (var client = new WebClient())
            {
                var datastream = client.OpenRead(url1);
                using (StreamReader reader = new StreamReader(datastream))
                {
                    StringBuilder sb = new StringBuilder();
                    while (!reader.EndOfStream)
                        sb.Append(reader.ReadLine());
                    var data = sb.ToString();
                    var index = data.IndexOf(ss, StringComparison.OrdinalIgnoreCase);
                    data = data.Substring(index, ss.Length + 12);
                    return data;
                }
            }
        }

        public override HtmlNode GetRatingNode(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var node = doc.GetElementbyId("dynamiccontent");
            var data1 = node.SelectNodes("div/span[2]/span[2]").First();
            return data1;
        }

        public override string GetRating(HtmlNode node)
        {
            var rating = node.InnerText;
            if (!string.IsNullOrWhiteSpace(rating))
                rating += "/5";
            return rating;
        }
    }
}