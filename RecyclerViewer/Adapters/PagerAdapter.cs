using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using static Android.Widget.TextView;
using MovieBuffLib;

namespace RecyclerViewer
{
    class PagerAdapter : FragmentStatePagerAdapter
    {
        string[] tabTitles;

        public override int Count
        {
            get
            {
                return tabTitles.Length;
            }
        }

        public PagerAdapter(Context context, Android.Support.V4.App.FragmentManager fm) : base(fm)
        {
            tabTitles = context.Resources.GetTextArray(Resource.Array.sections);
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(tabTitles[position]);
        }

        public View GetTabView(ViewGroup parent, int position)
        {
            View tab = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.custom_tab, null);
            TextView tv = (TextView)tab.FindViewById(Resource.Id.custom_text);
            tv.SetText(tabTitles[position], BufferType.Normal);
            return tab;
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            switch (position)
            {
                //case 0:
                //    return DashboardFragment.NewInstance();
                case 0:
                    return MoviesFragment.NewInstance(MovieListType.NowPlaying);
                case 1:
                    return MoviesFragment.NewInstance(MovieListType.Upcoming);
                case 2:
                    return MoviesFragment.NewInstance(MovieListType.Trending);
            }
            return null;
        }

        public override int GetItemPosition(Java.Lang.Object frag)
        {
            return PositionNone;
        }
    }

    class MoviePagerAdapter : FragmentStatePagerAdapter
    {
        string[] tabTitles;
        string movieName;
        int movieId;
        int imdbId;

        public override int Count
        {
            get
            {
                return tabTitles.Length;
            }
        }

        public MoviePagerAdapter(Context context, Android.Support.V4.App.FragmentManager fm, string movieName, int movieId, int imdbId) : base(fm)
        {
            tabTitles = context.Resources.GetTextArray(Resource.Array.movieDetails);
            this.movieName = movieName;
            this.movieId = movieId;
            this.imdbId = imdbId;
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(tabTitles[position]);
        }

        public View GetTabView(ViewGroup parent, int position)
        {
            View tab = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.custom_tab, null);
            TextView tv = (TextView)tab.FindViewById(Resource.Id.custom_text);
            tv.SetText(tabTitles[position], BufferType.Normal);
            return tab;
        }
        bool summary = false;
        bool trailer = false;
        bool reviews = false;
        bool cast = false;
        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return SummaryFragment.NewInstance(movieName, movieId);
                case 1:
                    return TrailerFragment.NewInstance(movieName, movieId);
                case 2:
                    return ReviewsFragment.NewInstance(movieName, movieId, imdbId);
                case 3:
                    return CastFragment.NewInstance(movieName, movieId);
            }
            return null;
        }

        public override int GetItemPosition(Java.Lang.Object frag)
        {
            return PositionNone;
        }
    }

    class CastPagerAdapter : FragmentStatePagerAdapter
    {
        string[] tabTitles;
        string castName;
        long castId;

        public override int Count
        {
            get
            {
                return tabTitles.Length;
            }
        }

        public CastPagerAdapter(Context context, Android.Support.V4.App.FragmentManager fm, string castName, long castId) : base(fm)
        {
            tabTitles = context.Resources.GetTextArray(Resource.Array.castDetails);
            this.castName = castName;
            this.castId = castId;
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(tabTitles[position]);
        }

        public View GetTabView(ViewGroup parent, int position)
        {
            View tab = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.custom_tab, null);
            TextView tv = (TextView)tab.FindViewById(Resource.Id.custom_text);
            tv.SetText(tabTitles[position], BufferType.Normal);
            return tab;
        }

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                //case 0:
                //    return new SummaryFragment(castName, 0, castId, true);
                case 0:
                    return CastFragment.NewInstance(castName, 0, castId, true);
            }
            return null;
        }

        public override int GetItemPosition(Java.Lang.Object frag)
        {
            return PositionNone;
        }
    }
}