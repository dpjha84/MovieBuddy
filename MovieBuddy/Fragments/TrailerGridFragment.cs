using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace MovieBuddy
{
    public class TrailerGridFragment : BaseFragment
    {
        MovieGridAdapter adapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static TrailerGridFragment NewInstance()
        {
            var frag2 = new TrailerGridFragment { Arguments = new Bundle() };
            return frag2;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

            RecyclerView rv = (RecyclerView)rootView.FindViewById(Resource.Id.rv_recycler_view);
            rv.HasFixedSize = true;

            var llm = new GridLayoutManager(this.Context, 1, GridLayoutManager.Vertical, false);
            rv.SetLayoutManager(llm);

            adapter = new MovieGridAdapter(MovieManager.Instance.GetAllTrailers(), true, true);
            adapter.ItemClick += OnItemClick;
            rv.SetAdapter(adapter);
            return rootView;
        }

        protected override void OnItemClick(object sender, int position)
        {
          //  var movie = (sender as MovieGridAdapter).mPhotoAlbum[position];
          //  Intent viewIntent =
          //new Intent("android.intent.action.VIEW",
          //  Uri.Parse($"https://www.youtube.com/embed/{movie.Trailer}"));
          //  StartActivity(viewIntent);
        }
    }
}