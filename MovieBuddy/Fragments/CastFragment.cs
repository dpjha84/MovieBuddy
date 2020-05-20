using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using System.Collections.Generic;

namespace MovieBuddy
{
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

        protected override List<TMDbLib.Objects.Search.SearchMovie> GetMovies()
        {
            var castId = Arguments.GetInt("castId");
            var movieList = new List<TMDbLib.Objects.Search.SearchMovie>();
            foreach (var item in MovieManager.Instance.GetMovieCredits(castId).Cast)
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

    public class CastFragment : BaseFragment
    {
        CastAdapter castAdapter;
        public static CastFragment NewInstance(int movieId)
        {
            var frag1 = new CastFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieId", movieId);
            frag1.Arguments = bundle;            
            return frag1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            movieId = Arguments.GetInt("movieId");
            View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

            var rv = (RecyclerView)rootView.FindViewById(Resource.Id.rv_recycler_view);
            rv.NestedScrollingEnabled = false;
            rv.HasFixedSize = true;

            var llm = new GridLayoutManager(this.Context, 3, GridLayoutManager.Vertical, false);
            rv.SetLayoutManager(llm);
            castAdapter = new CastAdapter(MovieManager.Instance.GetCastAndCrew(movieId).Cast);
            castAdapter.ItemClick += OnItemClick;
            rv.SetAdapter(castAdapter);
            return rootView;
        }

        protected override void OnItemClick(object sender, int position)
        {
            Intent intent = new Intent(this.Context, typeof(CastInfoActivity));
            Bundle b = new Bundle();
            b.PutInt("castId", (sender as CastAdapter).Cast[position].Id);
            b.PutString("castName", (sender as CastAdapter).Cast[position].Name);
            var cast = (sender as CastAdapter).Cast[position];
            var backdrop = cast.ProfilePath;
            b.PutString("imageUrl", !string.IsNullOrWhiteSpace(backdrop) ? backdrop : cast.ProfilePath);
            intent.PutExtras(b);
            StartActivity(intent);
        }

        public override void OnDestroy()
        {
            if(castAdapter != null)
                castAdapter.ItemClick -= OnItemClick;
            base.OnDestroy();
        }
    }
}