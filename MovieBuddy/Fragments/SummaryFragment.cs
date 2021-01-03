using Android.OS;
using Android.Support.V7.Widget;
using Refractored.Fab;
using System.Collections.Generic;

namespace MovieBuddy
{
    public class SummaryFragment : RecyclerViewFragment
    {
        protected bool ShowRatingBar = false;
        public static SummaryFragment NewInstance(string name, int movieId)
        {
            var frag1 = new SummaryFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieId", movieId);
            bundle.PutString("name", name);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override int SpanCount { get => 1; }

        protected virtual Dictionary<string, string> GetContent()
        {
            ShowRatingBar = true;
            return MovieManager.Instance.GetFullOverview(MovieId);
        }

        protected override RecyclerView.Adapter SetAdapter()
        {
            //var list = root.FindViewById<ListView>(Android.Resource.Id.List);
            //var adapter = new ListViewAdapter(Activity, Resources.GetStringArray(Resource.Array.countries));
            //list.Adapter = adapter;

            //var fab = root.FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.AttachToListView(list, this, this);
            //fab.Click += (sender, args) =>
            //{
            //    Toast.MakeText(Activity, "FAB Clicked!", ToastLength.Short).Show();
            //};

            adapter = new MovieSummaryAdapter(GetContent(), ShowRatingBar);
            
            return adapter;
        }
    }
}