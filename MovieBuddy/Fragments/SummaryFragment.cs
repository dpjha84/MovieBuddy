using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieBuddy
{
    public class ReviewFragment : SummaryFragment
    {
        public static new ReviewFragment NewInstance(string name, int Id)
        {
            var frag1 = new ReviewFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("Id", Id);
            bundle.PutString("name", name);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override List<string> GetContent()
        {
            var result = new List<string>();
            var reviews = MovieManager.Instance.GetReviews(movieId);
            foreach (var review in reviews)
            {
                result.Add(review.Author + '\n');
                result.Add(review.Content + '\n' + '\n');
                //sb.AppendLine(review.Author);
                //sb.AppendLine();
                //sb.AppendLine(review.Content);
                //sb.AppendLine();
                //sb.AppendLine(review.Url);
                //sb.AppendLine();
                //sb.AppendLine();
            }
            return result;
        }
    }

    public class SummaryFragment : BaseFragment
    {
        MovieSummaryAdapter adapter;
        public static SummaryFragment NewInstance(string name, int Id)
        {
            var frag1 = new SummaryFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("Id", Id);
            bundle.PutString("name", name);
            frag1.Arguments = bundle;
            return frag1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            movieName = Arguments.GetString("name");
            movieId = Arguments.GetInt("Id");
            View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

            RecyclerView rv = (RecyclerView)rootView.FindViewById(Resource.Id.rv_recycler_view);
            rv.NestedScrollingEnabled = false;
            var llm = new LinearLayoutManager(this.Context);
            rv.SetLayoutManager(llm);

            adapter = new MovieSummaryAdapter(GetContent());
            rv.SetAdapter(adapter);
            return rootView;
        }

        protected virtual List<string> GetContent()
        {
            return MovieManager.Instance.GetFullOverview(movieId);
        }

        public override void OnDestroy()
        {
            if (adapter != null)
                adapter.ItemClick -= OnItemClick;
            base.OnDestroy();
        }
    }
}