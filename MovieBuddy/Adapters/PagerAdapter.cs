using Android.Content;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using static Android.Widget.TextView;

namespace MovieBuddy
{
    public abstract class PagerAdapter : FragmentStatePagerAdapter
    {
        protected string[] tabTitles;

        public override int Count => tabTitles.Length;

        public PagerAdapter(Context context, int arrayId, FragmentManager fm) : base(fm)
        {
            tabTitles = context.Resources.GetTextArray(arrayId);
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(tabTitles[position]);
        }

        public override int GetItemPosition(Java.Lang.Object frag)
        {
            return PositionNone;
        }

        public View GetTabView(ViewGroup parent, int position)
        {
            View tab = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.custom_tab, null);
            TextView tv = (TextView)tab.FindViewById(Resource.Id.custom_text);
            tv.SetText(tabTitles[position], BufferType.Normal);
            return tab;
        }
    }

    internal class HomePagerAdapter : PagerAdapter
    {
        public HomePagerAdapter(Context context, FragmentManager fm) : base(context, Resource.Array.sections, fm) { }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return VideosFragment.NewInstance();
                case 1:
                    return MoviesFragment.NewInstance(MovieListType.NowPlaying);
                case 2:
                    return MoviesFragment.NewInstance(MovieListType.Upcoming);
                //case 2:
                //    return MoviesFragment.NewInstance(MovieListType.Trending);
                case 3:
                    return MoviesFragment.NewInstance(MovieListType.Popular);
                case 4:
                    return TmdbTopRatedMoviesFragment.NewInstance();
                case 5:
                    return ImdbMoviesFragment.NewInstance();
                default:
                    return null;
            }
        }
    }

    internal class MoviePagerAdapter : PagerAdapter
    {
        private readonly string movieName;
        private readonly int movieId;
        private readonly string backdrop;
        private readonly string releaseDate;
        private readonly string lang;

        public MoviePagerAdapter(Context context, FragmentManager fm, string movieName, int movieId, string backdrop, string releaseDate, string lang)
            : base(context, Resource.Array.movieDetails, fm)
        {
            this.movieName = movieName;
            this.movieId = movieId;
            this.backdrop = backdrop;
            this.releaseDate = releaseDate;
            this.lang = lang;
        }

        public override Fragment GetItem(int position)
        {
            return position switch
            {
                0 => SummaryFragment.NewInstance(movieName, movieId),
                1 => CastFragment.NewInstance(movieId),
                2 => VideosFragment.NewInstance(movieName, movieId, releaseDate, lang),
                //3 => TrailerFragment.NewInstance(movieName, movieId, backdrop),
                3 => ReviewFragment.NewInstance(movieName, movieId),
                4 => SimilarMoviesFragment.NewInstance(movieId),
                _ => null,
            };
        }
    }

    internal class SearchPagerAdapter : PagerAdapter
    {
        private readonly string query;

        public SearchPagerAdapter(Context context, FragmentManager fm, string query)
            : base(context, Resource.Array.searchDetails, fm)
        {
            this.query = query;
        }

        public override Fragment GetItem(int position)
        {
            return position switch
            {
                0 => SearchMoviesFragment.NewInstance(query),
                1 => SearchPersonFragment.NewInstance(query),
                _ => null,
            };
        }
    }

    internal class CastPagerAdapter : PagerAdapter
    {
        private readonly string castName;
        private readonly int castId;

        public CastPagerAdapter(Context context, FragmentManager fm, string castName, int castId)
            : base(context, Resource.Array.castDetails, fm)
        {
            this.castName = castName;
            this.castId = castId;
        }

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return CastBioFragment.NewInstance(castId);
                case 1:
                    return CastMoviesFragment.NewInstance(castId);
            }
            return null;
        }
    }
}