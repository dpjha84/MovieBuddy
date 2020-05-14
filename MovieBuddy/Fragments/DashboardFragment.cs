using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;

namespace MovieBuddy
{
    public class DashboardFragment : BaseFragment
    {
        DashboardGridAdapter adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static DashboardFragment NewInstance()
        {
            var frag2 = new DashboardFragment { Arguments = new Bundle() };
            return frag2;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

            RecyclerView rv = (RecyclerView)rootView.FindViewById(Resource.Id.rv_recycler_view);
            rv.HasFixedSize = true;

            var llm = new LinearLayoutManager(this.Context, LinearLayoutManager.Vertical, false);
            rv.SetLayoutManager(llm);
            rv.AddItemDecoration(new DividerItemDecoration(this.Context, llm.Orientation));

            //var releasedAll = MovieManager.Instance.GetReleased();
            //var upcomingAll = MovieManager.Instance.GetUpcoming();
            //var released = new MovieList { Latest = releasedAll.Where(l => l.ReleaseDate == releasedAll[0].ReleaseDate).ToList(), Others = releasedAll.Where(l => l.ReleaseDate < releasedAll[0].ReleaseDate).ToList() };
            //var upcoming = new MovieList { Latest = upcomingAll.Where(l => l.ReleaseDate == upcomingAll[0].ReleaseDate).ToList(), Others = upcomingAll.Where(l => l.ReleaseDate > upcomingAll[0].ReleaseDate).ToList() };
            var upcoming = new MovieList();
            var released = new MovieList(); //MovieManager.Instance.Upcoming;
            adapter = new DashboardGridAdapter(new List<MovieList> { released, upcoming }, true, true);
            adapter.ItemClick += OnItemClick;
            adapter.MoreClick += MoreClick;
            rv.SetAdapter(adapter);
            return rootView;
        }

        private void MoreClick(object sender, int position)
        {
            ViewPager pager = ((Activity)this.Context).FindViewById<ViewPager>(Resource.Id.viewpager);
            pager.SetCurrentItem(position + 1, true);
        }

        protected override void OnItemClick(object sender, int position)
        {

            //  var movie = (sender as DashboardGridAdapter).mPhotoAlbum[position];
            //  Intent viewIntent =
            //new Intent("android.intent.action.VIEW",
            //  Uri.Parse($"https://www.youtube.com/embed/{movie.Trailer}"));
            //  StartActivity(viewIntent);
        }
    }

    public class MovieList
    {
        public List<TmdbMovie> Latest { get; set; }
        public List<TmdbMovie> Others { get; set; }
    }
}