using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMDbLib.Objects.Reviews;

//using TMDbLib.Objects.General;
//using TMDbLib.Objects.Movies;

namespace MovieBuddy
{
    public class Movie
    {
        public int Id { get; set; }

        public Dictionary<string, string> Summary { get; set; }

        public List<int> Casts { get; set; }

        public List<string> Videos { get; set; }

        public List<ReviewBase> Reviews { get; set; }

        public List<int> Similar { get; set; }
    }
    public class YouTubeApiListId
    {
        public string VideoId { get; set; }
    }

    public class YouTubeApiListItem
    {
        public YouTubeApiListId Id { get; set; }
    }

    public class YouTubeApiListRoot
    {
        public List<YouTubeApiListItem> Items { get; set; }
    }

    public class SearchMovie : SearchMovieTvBase
    {
        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }

        [JsonProperty("release_date")]
        public DateTime? ReleaseDate { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("video")]
        public bool Video { get; set; }

        public static SearchMovie FromJson(string json) => JsonConvert.DeserializeObject<SearchMovie>(json, Converter.Settings);
    }

    public class SearchMovieTvBase : SearchBase
    {
        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("genre_ids")]
        public List<int> GenreIds { get; set; }

        [JsonProperty("original_language")]
        public string OriginalLanguage { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }
    }

    public class SearchBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonIgnore]
        [JsonProperty("media_type")]
        public MediaType MediaType { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }
    }

    public enum MediaType
    {
        Unknown = 0,
        Movie = 1,
        Tv = 2,
        Person = 3
    }

    //public class MovieData
    //{
    //    public int Id { get; set; }
    //    public SearchMovie Summary { get; set; }
    //    public string Trailor { get; set; }
    //    public CastDetail Cast { get; set; }
    //}

    public partial class CastDetail
    {
        [JsonProperty("cast")]
        public List<Cast> Cast { get; set; }

        [JsonProperty("crew")]
        public List<Crew> Crew { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public partial class Credits
    {
        [JsonProperty("cast")]
        public List<Cast> Cast { get; set; }

        [JsonProperty("crew")]
        public List<Crew> Crew { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public partial class Crew
    {
        [JsonProperty("credit_id")]
        public string CreditId { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("gender")]
        public long Gender { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("job")]
        public string Job { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }
    }

    public partial class Cast
    {
        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("character")]
        public string Character { get; set; }

        [JsonProperty("credit_id")]
        public string CreditId { get; set; }

        [JsonProperty("genre_ids")]
        public List<long> GenreIds { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("original_language")]
        public string OriginalLanguage { get; set; }

        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("video")]
        public bool Video { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public long VoteCount { get; set; }
    }

    public partial class Cast
    {
        [JsonProperty("cast_id")]
        public long CastId { get; set; }

        [JsonProperty("gender")]
        public long Gender { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("order")]
        public long Order { get; set; }

        [JsonProperty("profile_path")]
        public string ProfilePath { get; set; }

        public static Cast FromJson(string json) => JsonConvert.DeserializeObject<Cast>(json, Converter.Settings);
    }

    public partial class CastDetail
    {
        public static CastDetail FromJson(string json) => JsonConvert.DeserializeObject<CastDetail>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this CastDetail self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }

    public partial class TmdbMovie
    {
        [JsonProperty("credits")]
        public Credits Credits { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("belongs_to_collection")]
        public BelongsToCollection BelongsToCollection { get; set; }

        [JsonProperty("budget")]
        public long Budget { get; set; }

        [JsonProperty("genres")]
        public Genre[] Genres { get; set; }

        [JsonProperty("homepage")]
        public string Homepage { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("imdb_id")]
        public string ImdbId { get; set; }

        [JsonProperty("original_language")]
        public string OriginalLanguage { get; set; }

        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("production_companies")]
        public Genre[] ProductionCompanies { get; set; }

        [JsonProperty("production_countries")]
        public ProductionCountry[] ProductionCountries { get; set; }

        [JsonProperty("release_date")]
        public DateTime? ReleaseDate { get; set; }

        [JsonProperty("revenue")]
        public long Revenue { get; set; }

        [JsonProperty("runtime")]
        public long Runtime { get; set; }

        [JsonProperty("spoken_languages")]
        public SpokenLanguage[] SpokenLanguages { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tagline")]
        public string Tagline { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("video")]
        public bool Video { get; set; }

        [JsonProperty("videos")]
        public ResultContainer<Video> Videos { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public long VoteCount { get; set; }

        [JsonProperty("trailer")]
        public string Trailer { get; set; }

        public string ToiRating { get; set; }

        public string HtRating { get; set; }

        public string ImdbRating { get; set; }

        public string GenreText { get; set; }

        public CastDetail Cast { get; set; }

        public static TmdbMovie FromJson(string json) => JsonConvert.DeserializeObject<TmdbMovie>(json, Converter.Settings);

        public static string ToJson(TmdbMovie movie) => JsonConvert.SerializeObject(movie, Converter.Settings);
    }

    public class Video
    {
        public Video()
        {
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("iso_3166_1")]
        public string Iso_3166_1 { get; set; }

        [JsonProperty("iso_639_1")]
        public string Iso_639_1 { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class ResultContainer<T>
    {
        public ResultContainer()
        {
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("results")]
        public List<T> Results { get; set; }
    }

    public partial class SpokenLanguage
    {
        [JsonProperty("iso_639_1")]
        public string Iso639_1 { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class ProductionCountry
    {
        [JsonProperty("iso_3166_1")]
        public string Iso3166_1 { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Genre
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class BelongsToCollection
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }
    }

    public partial class QueryMovie
    {
        [JsonProperty("page")]
        public long Page { get; set; }

        [JsonProperty("total_results")]
        public long TotalResults { get; set; }

        [JsonProperty("total_pages")]
        public long TotalPages { get; set; }

        [JsonProperty("results")]
        public TmdbMovie[] Results { get; set; }
    }

    //public partial class Result
    //{
    //    [JsonProperty("vote_count")]
    //    public long VoteCount { get; set; }

    //    [JsonProperty("id")]
    //    public long Id { get; set; }

    //    [JsonProperty("video")]
    //    public bool Video { get; set; }

    //    [JsonProperty("vote_average")]
    //    public long VoteAverage { get; set; }

    //    [JsonProperty("title")]
    //    public string Title { get; set; }

    //    [JsonProperty("popularity")]
    //    public double Popularity { get; set; }

    //    [JsonProperty("poster_path")]
    //    public string PosterPath { get; set; }

    //    [JsonProperty("original_language")]
    //    public string OriginalLanguage { get; set; }

    //    [JsonProperty("original_title")]
    //    public string OriginalTitle { get; set; }

    //    [JsonProperty("genre_ids")]
    //    public object[] GenreIds { get; set; }

    //    [JsonProperty("backdrop_path")]
    //    public object BackdropPath { get; set; }

    //    [JsonProperty("adult")]
    //    public bool Adult { get; set; }

    //    [JsonProperty("overview")]
    //    public string Overview { get; set; }

    //    [JsonProperty("release_date")]
    //    public DateTime ReleaseDate { get; set; }
    //}

    public partial class QueryMovie
    {
        public static QueryMovie FromJson(string json) => JsonConvert.DeserializeObject<QueryMovie>(json, Converter.Settings);
    }
}