using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using MovieBuffLib;
using System;
using System.Collections.Generic;

namespace RecyclerViewer
{
    public class CastFragment : BaseFragment
    {
        //bool isMovie = false;
        //long castId = 0;
        MovieGridAdapter movieAdapter;
        CastAdapter castAdapter;
        public static CastFragment NewInstance(string name, int Id, long castId = 0, bool isMovie = false)
        {
            var frag1 = new CastFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("Id", Id);
            bundle.PutString("name", name);
            bundle.PutLong("castId", castId);
            bundle.PutBoolean("isMovie", isMovie);
            frag1.Arguments = bundle;            
            return frag1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            movieName = Arguments.GetString("name");
            movieId = Arguments.GetInt("Id");
            long castId = Arguments.GetLong("castId");
            bool isCast = Arguments.GetBoolean("isMovie");
            View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

            var rv = (RecyclerView)rootView.FindViewById(Resource.Id.rv_recycler_view);            
            rv.HasFixedSize = true;

            var llm = new GridLayoutManager(this.Context, 2, GridLayoutManager.Vertical, false);
            rv.SetLayoutManager(llm);
            //rv.AddItemDecoration(new DividerItemDecoration(rv.Context, llm.Orientation));

            if (Arguments.GetBoolean("isMovie"))
            {
                var movieList = new List<TmdbMovie>();
                foreach (var item in MovieManager.Instance.GetMovieCredits(castId).Cast)
                    movieList.Add(new TmdbMovie
                    {
                        Id = item.Id,
                        Title = item.Title,
                        OriginalTitle = item.OriginalTitle,
                        BackdropPath = item.BackdropPath,
                        PosterPath = item.PosterPath,
                        ReleaseDate = string.IsNullOrWhiteSpace(item.ReleaseDate) ? DateTime.MinValue : DateTime.Parse(item.ReleaseDate),
                        Overview = item.Overview,
                        OriginalLanguage = item.OriginalLanguage
                    });
                movieAdapter = new MovieGridAdapter(movieList);
                movieAdapter.ItemClick += OnItemClick;
                rv.SetAdapter(movieAdapter);
            }
            else
            {
                castAdapter = new CastAdapter(MovieManager.Instance.GetCast(movieId, movieName).Cast);
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
                b.PutLong("castId", (sender as CastAdapter).Cast[position].Id);
                b.PutString("castName", (sender as CastAdapter).Cast[position].Name);
                var cast = (sender as CastAdapter).Cast[position];
                var backdrop = cast.BackdropPath;
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