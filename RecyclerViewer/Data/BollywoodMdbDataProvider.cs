using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using TMDbLib.Objects.Search;
using System.Net;
using System.IO;
using TMDbLib.Objects.General;
using System.Text;
using Newtonsoft.Json;
using RecyclerViewer.Data;

namespace RecyclerViewer
{
    public class BollywoodMdbDataProvider : IMovieDataProvider
    {
        public string GetSummary(string name)
        {
            var movie = new IMDb_Scraper.IMDb(name);
            movie.ParseIMDbPage();
            var url = $"http://www.imdb.com/title/{movie.Id}";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            var node1 = GetNode(doc, "div[3]/div[2]/div[1]/div[1]");
            var node2 = GetNode(doc, "div[3]/div[1]/div[1]");
            //var node2 = doc.GetElementbyId("title-overview-widget").SelectNodes("div[3]/div[1]/div[1]").First();
            //var node = doc.DocumentNode.SelectSingleNode(.SelectNodes("[@id='title - overview - widget']/div[3]/div[2]/div[1]/div[1]").First();
            if (node1 == null && node2 != null)
                return node2.InnerHtml.Trim();
            else if (node2 == null && node1 != null)
                return node1.InnerHtml.Trim();
            else if (node2 == null && node1 == null)
                return null;
            else
            {
                var node = node1.InnerText.Length > node2.InnerText.Length ? node1 : node2;
                return node.InnerHtml.Trim();
            }
        }

        private HtmlNode GetWrapperNode(HtmlDocument doc, string element, string xpath)
        {
            try
            {
                return doc.GetElementbyId(element).SelectNodes(xpath).First();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private HtmlNode GetNode(HtmlDocument doc, string xpath)
        {
            try
            {
                return doc.GetElementbyId("title-overview-widget").SelectNodes(xpath).First();
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public List<SearchMovie> GetData(int year, int month)
        {
            var bmdbMovies = new Dictionary<string, SearchMovie>(StringComparer.InvariantCultureIgnoreCase);
            var movies1 = new List<SearchMovie>();
            var url = $"http://www.bollywoodmdb.com/movies/bollywood-hindi-movies-list-of-{year}-1";
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var monthName = new DateTime(2000, month, 1).ToString("MMM");

            var movieData = doc.GetElementbyId(monthName).SelectNodes("div/div").First();
            foreach (var item in movieData.ChildNodes)
            {
                if (item.Name != "div") continue;
                try
                {
                    var node = item.SelectNodes("div/a").First();
                    var data = node.GetAttributeValue("title", "").Split(';');
                    var name = data[0];
                    var date = data[1].Trim().Replace("Release Date: ", "");
                    var imgNode = node.SelectNodes("img").First();
                    var url1 = imgNode.GetAttributeValue("src", "");
                    if (string.IsNullOrWhiteSpace(url1))
                        url1 = imgNode.GetAttributeValue("data-src", "");

                    var dt = DateTime.Parse(date);
                    if (!MovieManager.IsMoviePresent(name) && dt.Month == month && dt.Year == year)
                    {
                        var href = node.GetAttributeValue("href", "");
                        //var trailorLink = GetTrailorLink(href);
                        bmdbMovies.Add(name, new SearchMovie
                        {
                            ReleaseDate = dt,
                            OriginalTitle = name,
                            Title = name,
                            PosterPath = url1
                            //Trailor = trailorLink
                            //Summary = GetSummary(name)
                        });
                    }
                }
                catch (Exception ex)
                { }
            }
            var output = new Dictionary<string, RecyclerViewer.QueryMovie>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in bmdbMovies.Values)
            {
                var movie = new TMdbDataProvider().FindMovie(item.OriginalTitle);
                output.Add(item.OriginalTitle, movie);
            }
            foreach (KeyValuePair<string, RecyclerViewer.QueryMovie> item in output)
            {
                if (item.Value.Results == null || item.Value.Results.Count() == 0)
                {
                    movies1.Add(bmdbMovies[item.Key]);
                }
                foreach (var item1 in item.Value.Results)
                {
                    if ((item.Key.Equals(item1.Title, StringComparison.InvariantCultureIgnoreCase) ||
                        item.Key.Equals(item1.OriginalTitle, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (item1.ReleaseDate != null && item1.ReleaseDate.Value.Year < 2017) continue;
                        item1.ReleaseDate = bmdbMovies[item.Key].ReleaseDate;
                        if(item1.ReleaseDate.Value.Year == year && item1.ReleaseDate.Value.Month == month)
                            movies1.Add(item1);
                    }
                }
            }
            return movies1;
        }

        private string GetTrailorLink(string href)
        {
            var web = new HtmlWeb();
            var doc = web.Load(href);
            var node = GetWrapperNode(doc, "wrapper", "div[2]/div/div[1]/div[1]/div[3]/div[2]/div[2]/div[5]/div[1]/a");
            if (node == null)
            {
                node = GetWrapperNode(doc, "wrapper", "div[2]/div/div[1]/div[1]/div[3]/div[2]/div[2]/div[6]/div[1]/a");
                if (node == null)
                    node = GetWrapperNode(doc, "wrapper", "div[2]/div/div[1]/div[1]/div[3]/div[2]/div[2]/div[7]/div[1]/a");
            }
            if (node == null) return null;

            var nextPage = node.GetAttributeValue("href", "");

            var web1 = new HtmlWeb();
            var doc1 = web1.Load(nextPage);
            var node1 = doc1.GetElementbyId("wrapper").SelectNodes("div[2]/div/div[1]/div[1]/div/div[4]/iframe").First();
            var youtubeUrl = node1.GetAttributeValue("src", "");
            return youtubeUrl;
        }

        public CastDetail GetCast(int movieId)
        {
            return new CastDetail { Cast = new List<Cast>() };
        }

        public string GetTrailer(int movieId)
        {
            return null;
        }
    }
}