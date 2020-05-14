using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieBuffLib;
using System.Collections.Generic;
using System.Diagnostics;

namespace MovieBuff.Test
{
    [TestClass]
    public class UnitTest1
    {
        private static List<TmdbMovie> movies2 = new List<TmdbMovie>();
        private static List<TmdbMovie> movies1 = new List<TmdbMovie>();
        private static List<TmdbMovie> movies = new List<TmdbMovie>();
        private static int successCount = 0;

        [ClassInitialize()]
        public static void TestMethod1(TestContext context)
        {
            MovieManager.Init(null);
            var sw = Stopwatch.StartNew();
            MovieManager.Instance.RefreshData();
            Debug.WriteLine($"Time taken - {sw.Elapsed}");
            movies1 = MovieManager.Instance.GetReleased();
            movies2 = MovieManager.Instance.GetUpcoming();
        }

        [TestMethod]
        public void VerifyCount()
        {
            Assert.IsTrue(movies2.Count > 0);
        }

        private static int attribCount = 0;

        [TestMethod]
        public void VerifyName()
        {
            //var data = new RealMovieManager(null).GetHtRating(null, 0, "Vodka Diaries");
            foreach (var movie in movies1)
            {
                attribCount += 8;
                Instrument(movie.Title, "Title");
                Instrument(movie.GenreText, "GenreText");
                Instrument(movie.ReleaseDate.ToString(), "ReleaseDate");
                Instrument(movie.Credits?.Cast, "Cast");
                Instrument(movie.Trailer, "Trailer");
                Instrument(movie.Overview, "overview");
                Instrument(movie.PosterPath, "PosterPath");
                Instrument(movie.BackdropPath, "BackdropPath");
                //if (movie.ReleaseDate <= DateTime.Now)
                //{
                //    attribCount += 3;
                //    Instrument(movie.ToiRating, "TOI Rating");
                //    Instrument(movie.HtRating, " HT Rating");
                //    Instrument(movie.ImdbRating, "IMDB Rating");
                //}
                Debug.WriteLine("");
                Assert.IsTrue(!string.IsNullOrWhiteSpace(movie.Title) &&
                    !string.IsNullOrWhiteSpace(movie.OriginalTitle));
            }
            foreach (var movie in movies2)
            {
                attribCount += 8;
                Instrument(movie.Title, "Title");
                Instrument(movie.GenreText, "GenreText");
                Instrument(movie.ReleaseDate.ToString(), "ReleaseDate");
                Instrument(movie.Credits?.Cast, "Cast");
                Instrument(movie.Trailer, "Trailer");
                Instrument(movie.Overview, "overview");
                Instrument(movie.PosterPath, "PosterPath");
                Instrument(movie.BackdropPath, "BackdropPath");
                //if (movie.ReleaseDate <= DateTime.Now)
                //{
                //    attribCount += 3;
                //    Instrument(movie.ToiRating, "TOI Rating");
                //    Instrument(movie.HtRating, " HT Rating");
                //    Instrument(movie.ImdbRating, "IMDB Rating");
                //}
                Debug.WriteLine("");
                Assert.IsTrue(!string.IsNullOrWhiteSpace(movie.Title) &&
                    !string.IsNullOrWhiteSpace(movie.OriginalTitle));
            }

            Debug.WriteLine(successCount);
            Debug.WriteLine(attribCount);
            Debug.WriteLine(((successCount * 100) / attribCount) + "%");
        }

        private void Instrument(List<Cast> list, string type)
        {
            if (list != null)
            {
                Debug.WriteLine("Cast present");
                successCount++;
            }
            else
            {
                Debug.WriteLine($"No {type}");
            }
        }

        private void Instrument(string attrib, string type)
        {
            if (!string.IsNullOrWhiteSpace(attrib))
            {
                Debug.WriteLine(attrib);
                successCount++;
            }
            else
            {
                Debug.WriteLine($"No {type}");
            }
        }
    }
}