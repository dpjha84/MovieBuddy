using Android.OS;
using System.Collections.Generic;

namespace MovieBuddy
{
    public class ReviewFragment : SummaryFragment
    {
        public static new ReviewFragment NewInstance(string name, int movieId)
        {
            var frag1 = new ReviewFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieId", movieId);
            bundle.PutString("name", name);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override Dictionary<string, string> GetContent()
        {
            var result = new Dictionary<string, string>();
            var reviews = MovieManager.Instance.GetReviews(MovieId);
            foreach (var review in reviews)
            {
                result.Add(review.Author, review.Content);
            }
            return result;
        }
    }
}