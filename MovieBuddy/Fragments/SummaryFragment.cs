using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using System;
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
            //(adapter as MovieSummaryAdapter).imdbImage.Click += ImdbImage_Click;
            return adapter;
        }

        private void ImdbImage_Click(object sender, System.EventArgs e)
        {
            try
            {
                //base.OnItemClick(sender, position);
                //var videoId = (sender as VideosAdapter).videos[position];
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse($"https://www.youtube.com/embed/{123}"));
                StartActivity(intent);
            }
            catch (Exception)
            {
                //Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }
    }
}