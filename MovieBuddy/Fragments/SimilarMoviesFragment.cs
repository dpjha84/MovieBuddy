using Android.OS;
using System.Collections.Generic;

namespace MovieBuddy
{
    public class SimilarMoviesFragment : MoviesFragment
    {
        public static SimilarMoviesFragment NewInstance(int movieId)
        {
            var frag1 = new SimilarMoviesFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieId", movieId);
            frag1.Arguments = bundle;
            return frag1;
        }

        int page = 1;
        protected override List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            return MovieManager.Instance.GetSimilar(MovieId, page++);
        }

        protected override void ResetPages() { }
    }

    public class ExploreMovieInfo
    {
        public int StartYear { get; set; }

        public int EndYear { get; set; }

        public string Genre { get; set; }

        //public string Language { get; set; }
    }

    public class ExploreMoviesFragment : MoviesFragment
    {
        public static ExploreMoviesFragment NewInstance(ExploreMovieInfo info)
        {
            var frag1 = new ExploreMoviesFragment();
            Bundle bundle = new Bundle();
            bundle.PutIntArray("exploreMovieInfo", new int[3]
            { info.StartYear, info.EndYear, info.Genre == "Any" ? -1 : MovieManager.GenreTextToIdMap[info.Genre]});
            frag1.Arguments = bundle;
            return frag1;
        }

        int page = 1;
        protected override List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            var info = Arguments.GetIntArray("exploreMovieInfo");
            if (info[0] > info[1]) return null;
            return MovieManager.Instance.ExploreMovies(info, page++);
        }

        protected override void ResetPages() { }
    }
}