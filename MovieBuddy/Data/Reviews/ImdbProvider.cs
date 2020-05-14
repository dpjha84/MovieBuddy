using HtmlAgilityPack;
using System.Linq;

namespace MovieBuddy
{
    public class ImdbProvider : ReviewProvider
    {
        public override string GetUrl(string name)
        {
            return $"http://www.imdb.com/title/{name}";
        }

        public override HtmlNode GetRatingNode(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var node = doc.GetElementbyId("title-overview-widget");
            var data = node.SelectNodes("div[2]/div[2]/div/div[1]/div[1]/div[1]/strong/span").First();
            return data;
        }

        public override string GetRating(HtmlNode node)
        {
            if (node == null || string.IsNullOrWhiteSpace(node.InnerText))
                return null;
            var rating = $"{node.InnerText}/10";
            return rating;
        }
    }
}