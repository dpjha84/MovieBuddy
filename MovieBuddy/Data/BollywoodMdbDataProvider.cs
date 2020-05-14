using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBuddy
{
    public class BollywoodMdbDataProvider : IMovieDataProvider
    {
        private HtmlDocument doc;
        YoutubeProvider youtubeProvider;

        public BollywoodMdbDataProvider()
        {
            var url = $"http://www.bollywoodmdb.com/movies/bollywood-hindi-movies-list-of-2018-1";
            var web = new HtmlWeb();
            doc = web.Load(url);
            youtubeProvider = new YoutubeProvider();
        }        

        public List<TmdbMovie> GetData(int year, int month)
        {
            var bmdbMovies = new Dictionary<string, TmdbMovie>(StringComparer.InvariantCultureIgnoreCase);
            var movies1 = new List<TmdbMovie>();

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
                    if (!MovieManager.Instance.IsMoviePresent(name) && dt.Month == month && dt.Year == year)
                    //if (dt.Month == month && dt.Year == year)
                    {
                        var href = node.GetAttributeValue("href", "");
                        //var trailorLink = GetTrailorLink(href);
                        bmdbMovies.Add(name, new TmdbMovie
                        {
                            ReleaseDate = dt,
                            OriginalTitle = name,
                            Title = name,
                            PosterPath = url1,
                            BackdropPath = url1
                            //Trailor = trailorLink
                            //Summary = GetSummary(name)
                        });
                    }
                }
                catch (Exception ex)
                {
                }
            }
            var output = new ConcurrentDictionary<string, QueryMovie>(StringComparer.InvariantCultureIgnoreCase);
            //foreach (var item in bmdbMovies.Values)
            Parallel.ForEach(bmdbMovies.Values, (item) =>
            {
                var movie = new TMdbDataProvider().FindMovie(item.OriginalTitle);
                //if (!movie.Results.Any(x => x.Title == item.Title) && item.OriginalTitle.Contains(" "))
                //    movie = new TMdbDataProvider().FindMovie(item.OriginalTitle.Replace(" ", ""));
                output.AddOrUpdate(item.OriginalTitle, movie, (s, queryMovie) => queryMovie);
            }
            );
            //foreach (KeyValuePair<string, QueryMovie> item in output)
            Parallel.ForEach(output, item =>
            {
                bool found = false;
                if (item.Value.Results == null || item.Value.Results.Count() == 0)
                {
                    bmdbMovies[item.Key].Trailer = youtubeProvider.GetTrailer(item.Key).Result;
                    movies1.Add(bmdbMovies[item.Key]);
                    found = true;
                }

                //foreach (var item1 in item.Value.Results)
                Parallel.ForEach(item.Value.Results, (item1) =>
                {
                    if (item.Key.Equals(item1.Title, StringComparison.InvariantCultureIgnoreCase) ||
                         item.Key.Equals(item1.OriginalTitle, StringComparison.InvariantCultureIgnoreCase) ||
                         item.Key.Replace(" ", "").Equals(item1.Title.Replace(" ", ""), StringComparison.InvariantCultureIgnoreCase) ||
                         item.Key.Replace(" ", "").Equals(item1.OriginalTitle.Replace(" ", ""), StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (item1.ReleaseDate != null && item1.ReleaseDate.Value.Year >= 2017)
                        {
                            item1.ReleaseDate = bmdbMovies[item.Key].ReleaseDate;
                            if (item1.ReleaseDate.Value.Year == year && item1.ReleaseDate.Value.Month == month)
                            {
                                var movie = new TMdbDataProvider().GetMovie(item1.Id);
                                movie.ReleaseDate = item1.ReleaseDate;
                                if (string.IsNullOrWhiteSpace(movie.PosterPath) &&
                                    !string.IsNullOrWhiteSpace(bmdbMovies[item.Key].PosterPath))
                                    movie.PosterPath = bmdbMovies[item.Key].PosterPath;
                                if (string.IsNullOrWhiteSpace(movie.BackdropPath) &&
                                    !string.IsNullOrWhiteSpace(bmdbMovies[item.Key].BackdropPath))
                                    movie.BackdropPath = bmdbMovies[item.Key].BackdropPath;
                                //movie.Credits = item1.Credits;
                                var trailer = movie.Videos;
                                movie.Trailer = trailer?.Results?.Count > 0 ? trailer.Results[0].Key : youtubeProvider.GetTrailer(item1.Title).Result;
                                movie.GenreText = string.Join(", ", movie?.Genres?.Select(g => g.Name));
                                movies1.Add(movie);
                                found = true;
                            }
                        }
                    }
                }
                );
                if (!found)
                {
                    bmdbMovies[item.Key].Trailer = youtubeProvider.GetTrailer(item.Key).Result;
                    movies1.Add(bmdbMovies[item.Key]);
                }
            }
            );
            return movies1;
        }
    }
}