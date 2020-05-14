using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MovieBuddy
{
    public class HtProvider : ReviewProvider
    {
        public override string GetUrl(string name)
        {
            var movieName1 = name.Replace(" ", "+");
            var movieName2 = name.Replace(" ", "-");
            var url1 = $"https://www.google.co.in/search?q={movieName1}+review";
            string ss = $"http://www.hindustantimes.com/movie-reviews/{movieName2}-movie-review-";
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
                    var lastIndex = data.IndexOf(".html", index);
                    data = data.Substring(index, lastIndex - index + 5);
                    return data;
                }
            }
        }

        public override HtmlNode GetRatingNode(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            HtmlNode data1;
            try
            {
                data1 = doc.DocumentNode.SelectNodes("/html/body/div[1]/section/div[1]/div/div[1]/article/div[2]/div[2]/p[1]/text()[3]").First();
            }
            catch (Exception)
            {
                data1 = doc.DocumentNode.SelectNodes("/html/body/div[1]/section/div/div/div[1]/article/div[2]/div[2]/p[2]/text()[3]").First();
            }
            return data1;
        }

        public override string GetRating(HtmlNode node)
        {
            var rating = node.InnerText;
            return rating;
        }
    }
}