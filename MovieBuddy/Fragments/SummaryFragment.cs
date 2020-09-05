using Android.OS;
using Android.Support.V7.Widget;
using System.Collections.Generic;

namespace MovieBuddy
{
    public class SummaryFragment : RecyclerViewFragment
    {
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
            return MovieManager.Instance.GetFullOverview(MovieId);
        }

        protected override RecyclerView.Adapter SetAdapter()
        {
            adapter = new MovieSummaryAdapter(GetContent());
            return adapter;
        }
    }
}