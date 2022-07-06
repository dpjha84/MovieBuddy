using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Languages;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Reviews;
using TMDbLib.Objects.Search;

namespace MovieBuddy
{
    public class TClient : TClientBase
    {
        private readonly TMDbClient tmdbClient;
        public const string tmdbApiKey = "c6b31d1cdad6a56a23f0c913e2482a31";
        public TClient()
        {
            tmdbClient = new TMDbClient(tmdbApiKey);

        }

        public string Key { get { return tmdbApiKey; } }

        public SearchMovie GetByImdbId(string imdbId)
        {
            base.Track();
            var data = tmdbClient.FindAsync(TMDbLib.Objects.Find.FindExternalSource.Imdb, imdbId).Result;
            return data.MovieResults[0];
        }

        public Task<SearchContainerWithDates<SearchMovie>> GetMovieNowPlayingListAsync(string language = null, int page = 0, string region = null, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetMovieNowPlayingListAsync(language, page, region, cancellationToken);
        }

        public Task<List<Genre>> GetMovieGenresAsync(CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetMovieGenresAsync(cancellationToken);
        }

        public Task<List<Language>> GetLanguagesAsync(CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetLanguagesAsync(cancellationToken);
        }

        public Task<SearchContainer<SearchMovie>> SearchMovieAsync(string query, int page = 0, bool includeAdult = false, int year = 0, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.SearchMovieAsync(query, page, includeAdult, year, cancellationToken);
        }

        public Task<SearchContainer<SearchPerson>> SearchPersonAsync(string query, int page = 0, bool includeAdult = false, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.SearchPersonAsync(query, page, includeAdult, cancellationToken);
        }

        public Task<SearchContainer<PersonResult>> GetPersonListAsync(PersonListType type, int page = 0, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetPersonListAsync(type, page, cancellationToken);
        }

        public Task<SearchContainerWithDates<SearchMovie>> GetMovieUpcomingListAsync(string language = null, int page = 0, string region = null, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetMovieUpcomingListAsync(language, page, region, cancellationToken);
        }

        public Task<SearchContainer<SearchMovie>> GetMoviePopularListAsync(string language = null, int page = 0, string region = null, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetMoviePopularListAsync(language, page, region, cancellationToken);
        }

        public Task<SearchContainer<SearchMovie>> GetMovieTopRatedListAsync(string language = null, int page = 0, string region = null, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetMovieTopRatedListAsync(language, page, region, cancellationToken);
        }

        public Task<SearchContainer<SearchMovie>> GetMovieSimilarAsync(int movieId, int page = 0, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetMovieSimilarAsync(movieId, page, cancellationToken);
        }

        public Task<SearchContainerWithId<ReviewBase>> GetMovieReviewsAsync(int movieId, int page = 0, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetMovieReviewsAsync(movieId, page, cancellationToken);
        }

        public Task<Credits> GetMovieCreditsAsync(int movieId)
        {
            base.Track();
            return tmdbClient.GetMovieCreditsAsync(movieId);
        }

        public Task<MovieCredits> GetPersonMovieCreditsAsync(int personId, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetPersonMovieCreditsAsync(personId, cancellationToken);
        }

        public Task<Movie> GetMovieAsync(int movieId, MovieMethods extraMethods = MovieMethods.Undefined, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetMovieAsync(movieId, extraMethods, cancellationToken);
        }

        public Task<ResultContainer<Video>> GetMovieVideosAsync(int movieId, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetMovieVideosAsync(movieId, cancellationToken);
        }

        public Task<Person> GetPersonAsync(int personId, PersonMethods extraMethods = PersonMethods.Undefined, CancellationToken cancellationToken = default)
        {
            base.Track();
            return tmdbClient.GetPersonAsync(personId, extraMethods, cancellationToken);
        }

        public List<SearchMovie> Discover(int startYear, int endYear, int genreId, int page = 0)
        {
            base.Track();
            var movies = new DiscoverMovie(tmdbClient);
            if (genreId != -1)
                movies = movies.IncludeWithAllOfGenre(new List<int> { genreId });

            for (int i = startYear; i <= endYear; i++)
            {
                movies = movies.WherePrimaryReleaseIsInYear(i);
            }
            return movies.Query(page).Result.Results;
        }

        //public SearchContainer<Movie> GetMoviesByUrl(string url, int page)
        //{
        //    url += $"page={page}&with_original_language={Globals.LanguageMap[Globals.SelectedLanguage]}";
        //    using (var client = new HttpClient())
        //    {
        //        var response = client.GetAsync(url);
        //        if (response.Result.IsSuccessStatusCode)
        //        {
        //            var data = response.Result.Content.ReadAsStringAsync();
        //            return JsonConvert.DeserializeObject<SearchContainer<Movie>>(data.Result);
        //        }
        //    }
        //    return null;
        //}
    }

    public class TClientBase
    {
        private static int calls = 0;

        public static void IncreaseCalls([CallerMemberName] string memberName = "")
        {
            Interlocked.Increment(ref calls);
            Debug.WriteLine($"*********************** {memberName} Call count: {calls}***********************");
        }
        protected void Track([CallerMemberName] string memberName = "")
        {
            Interlocked.Increment(ref calls);
            Debug.WriteLine($"*********************** {memberName} Call count: {calls}***********************");
            //Toast.MakeText(null, $"Call count: {calls}", ToastLength.Short);
        }

        public static void LogError(Exception ex, [CallerMemberName] string memberName = "")
        {
            Debug.WriteLine($"*********************** {memberName} Exception: {ex}***********************");
        }
    }
}
