using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using System.Collections.Generic;

namespace MovieBuddy
{
    public class CastFragment : BaseFragment
    {
        //bool isMovie = false;
        //long castId = 0;
        MovieGridAdapter movieAdapter;
        CastAdapter castAdapter;
        public static CastFragment NewInstance(string name, int Id, int castId = 0, bool isMovie = false)
        {
            var frag1 = new CastFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("Id", Id);
            bundle.PutString("name", name);
            bundle.PutInt("castId", castId);
            bundle.PutBoolean("isMovie", isMovie);
            frag1.Arguments = bundle;            
            return frag1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            movieName = Arguments.GetString("name");
            movieId = Arguments.GetInt("Id");
            int castId = Arguments.GetInt("castId");
            bool isCast = Arguments.GetBoolean("isMovie");
            View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

            var rv = (RecyclerView)rootView.FindViewById(Resource.Id.rv_recycler_view);            
            rv.HasFixedSize = true;

            var llm = new GridLayoutManager(this.Context, 3, GridLayoutManager.Vertical, false);
            rv.SetLayoutManager(llm);
            //rv.AddItemDecoration(new DividerItemDecoration(rv.Context, llm.Orientation));

            if (Arguments.GetBoolean("isMovie"))
            {
                var movieList = new List<TMDbLib.Objects.Search.SearchMovie>();
                //int castId = Arguments.GetInt("castId");
                foreach (var item in MovieManager.Instance.GetMovieCredits(castId).Cast)
                    movieList.Add(new TMDbLib.Objects.Search.SearchMovie
                    {
                        Id = item.Id,
                        Title = item.Title,
                        OriginalTitle = item.OriginalTitle,
                        BackdropPath = item.PosterPath,
                        PosterPath = item.PosterPath,
                        ReleaseDate = item.ReleaseDate,//string.IsNullOrWhiteSpace(item.ReleaseDate) ? DateTime.MinValue : DateTime.Parse(item.ReleaseDate),
                        Overview = item.Title,
                        //OriginalLanguage = item.OriginalLanguage
                    });
                movieAdapter = new MovieGridAdapter(movieList);
                movieAdapter.ItemClick += OnItemClick;
                rv.SetAdapter(movieAdapter);
            }
            else
            {
                castAdapter = new CastAdapter(MovieManager.Instance.GetCastAndCrew(movieId).Cast);
                castAdapter.ItemClick += OnItemClick;
                rv.SetAdapter(castAdapter);
            }
            return rootView;
        }

        protected override void OnItemClick(object sender, int position)
        {
            if (Arguments.GetBoolean("isMovie"))
            {
                Intent intent = new Intent(this.Context, typeof(MovieInfoActivity));
                Bundle b = new Bundle();
                b.PutInt("movieId", (int)(sender as MovieGridAdapter).mPhotoAlbum[position].Id);
                b.PutString("movieName", (sender as MovieGridAdapter).mPhotoAlbum[position].OriginalTitle);
                var movie = (sender as MovieGridAdapter).mPhotoAlbum[position];
                var backdrop = movie.BackdropPath;
                b.PutString("imageUrl", !string.IsNullOrWhiteSpace(backdrop) ? backdrop : movie.PosterPath);
                intent.PutExtras(b);
                StartActivity(intent);
                
            }
            else
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
        }

        //public override void OnDestroy()
        //{
        //    //movieAdapter.ItemClick -= OnItemClick;
        //    //castAdapter.ItemClick -= OnItemClick;
        //}

    }
}