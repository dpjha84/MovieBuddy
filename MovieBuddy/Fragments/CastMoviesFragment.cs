using Android.OS;
using System.Collections.Generic;
using TSearchMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy
{
    public class CastBioFragment : SummaryFragment
    {
        public static new CastBioFragment NewInstance(int castId)
        {
            var frag1 = new CastBioFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("castId", castId);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override Dictionary<string, string> GetContent()
        {
            var result = new Dictionary<string, string>();
            var person = MovieManager.Instance.GetPerson(CastId);
            if (person.Birthday.HasValue)
            {
                result.Add("Birthday", person.Birthday.Value.ToString("dd MMMM yyyy"));
                //result.Add(person.Birthday.Value.ToString("MMMM dd"));
                //result.Add("");
            }
            if (!string.IsNullOrWhiteSpace(person.PlaceOfBirth))
            {
                result.Add("Place of Birth", person.PlaceOfBirth);
                //result.Add(person.PlaceOfBirth);
                //result.Add("");
            }
            result.Add("Biography", person.Biography);
            //result.Add(person.Biography);
            return result;
        }
    }
    public class CastMoviesFragment : MoviesFragment
    {
        public static CastMoviesFragment NewInstance(int castId)
        {
            var frag1 = new CastMoviesFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("castId", castId);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override ClickableWithPagingAdapter<TSearchMovie> GetAdapter() => new CastMoviesAdapter();
        
        int page = 1;
        protected override List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            var data = MovieManager.Instance.GetMovieCredits(CastId, page++);
            if (data == null) return null;
            var movieList = new List<TMDbLib.Objects.Search.SearchMovie>();
            foreach (var item in data)
                movieList.Add(new TMDbLib.Objects.Search.SearchMovie
                {
                    Id = item.Id,
                    Title = item.Title,
                    OriginalTitle = item.OriginalTitle,
                    BackdropPath = item.PosterPath,
                    PosterPath = item.PosterPath,
                    ReleaseDate = item.ReleaseDate,
                    Overview = item.Title
                });
            return movieList;
        }
    }
}