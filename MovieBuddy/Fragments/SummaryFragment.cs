using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Linq;
using System.Text;

namespace MovieBuddy
{
    public class ReviewFragment : SummaryFragment
    {
        public static new ReviewFragment NewInstance(string name, int Id, long castId = 0, bool isCast = false)
        {
            var frag1 = new ReviewFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("Id", Id);
            bundle.PutString("name", name);
            bundle.PutLong("castId", castId);
            bundle.PutBoolean("isCast", isCast);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override string GetContent()
        {
            var sb = new StringBuilder();
            var reviews = MovieManager.Instance.GetReviews(movieId);
            foreach (var review in reviews)
            {
                sb.AppendLine(review.Author);
                sb.AppendLine();
                sb.AppendLine(review.Content);
                sb.AppendLine();
                sb.AppendLine(review.Url);
                sb.AppendLine();
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
    public class SummaryFragment : BaseFragment
    {
        public static SummaryFragment NewInstance(string name, int Id, long castId = 0, bool isCast = false)
        {
            var frag1 = new SummaryFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("Id", Id);
            bundle.PutString("name", name);
            bundle.PutLong("castId", castId);
            bundle.PutBoolean("isCast", isCast);
            frag1.Arguments = bundle;
            return frag1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            movieName = Arguments.GetString("name");
            movieId = Arguments.GetInt("Id");
            int Cast = Arguments.GetInt("castId");
            bool isCast = Arguments.GetBoolean("isCast");
            View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

            RecyclerView rv = (RecyclerView)rootView.FindViewById(Resource.Id.rv_recycler_view);
            //rv.HasFixedSize = true;
            //var llm = new GridLayoutManager(this.Context, 1, GridLayoutManager.Vertical, false);
            var llm = new LinearLayoutManager(this.Context);
            rv.SetLayoutManager(llm);

            //var summary = isCast ? MovieManager.GetMovieCredits(castId).Cast.First(c => c.Id == Id).Overview : MovieManager.FindMovie(movieName).Overview;
            //var summary = isCast ? MovieManager.Instance.GetMovieCredits(castId).Cast.First(c => c.Id == Id).Overview : MovieManager.Instance.GetFullOverview(movieId, movieName);
            MovieSummaryAdapter adapter = new MovieSummaryAdapter(GetContent());
            rv.SetAdapter(adapter);
            return rootView;
        }

        protected virtual string GetContent() => MovieManager.Instance.GetFullOverview(movieId);
    }
}