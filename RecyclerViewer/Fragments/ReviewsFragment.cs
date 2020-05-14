using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using MovieBuffLib;
using System;
using System.Collections.Generic;

namespace RecyclerViewer
{
    public class ReviewsFragment : BaseFragment
    {
        MovieReviewsAdapter adapter;
        public static ReviewsFragment NewInstance(string movieName, int movieId, int imdbId)
        {
            var frag1 = new ReviewsFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieId", movieId);
            bundle.PutString("movieName", movieName);
            bundle.PutInt("imdbId", imdbId);
            frag1.Arguments = bundle;
            return frag1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            movieName = Arguments.GetString("movieName");
            movieId = Arguments.GetInt("movieId");
            int imdbId = Arguments.GetInt("imdbId");

            View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

            RecyclerView rv = (RecyclerView)rootView.FindViewById(Resource.Id.rv_recycler_view);
            rv.HasFixedSize = true;

            var llm = new GridLayoutManager(this.Context, 3, GridLayoutManager.Vertical, false);
            rv.SetLayoutManager(llm);

            //var movie = MovieManager.Instance.GetMovie(movieId, movieName);
            //var reviews = new List<Review>();
            //if (movie?.ReleaseDate <= DateTime.Now)
            //{
            //    var key = (movieId == 0) ? movie.Title.Replace(" ", "") : movieId.ToString();
            //    reviews.Add(new Review
            //    {
            //        Reviewer = "Times of India",
            //        Text = "Nice Movie",
            //        Rating = MovieManager.Instance.GetToiRating(key),
            //        Image = Resource.Drawable.toi2
            //    });
            //    reviews.Add(new Review
            //    {
            //        Reviewer = "Hindustan Times",
            //        Text = "Nice Movie",
            //        Rating = MovieManager.Instance.GetHtRating(key),
            //        Image = Resource.Drawable.ht
            //    });
            //    reviews.Add(new Review
            //    {
            //        Reviewer = "IMDB",
            //        Text = "Average Movie",
            //        Rating = MovieManager.Instance.GetImdbRating(key),
            //        Image = Resource.Drawable.imdb
            //    });
            //}
            //var reviews = new DummyDataProvider().GetReviews(movieId, movieName, movie?.ImdbId, movie?.ReleaseDate > DateTime.Now).Result;
            //MovieReviewsAdapter adapter = new MovieReviewsAdapter(reviews);
            //adapter.ItemClick += OnItemClick;
            //rv.SetAdapter(adapter);
            return rootView;
        }

        //public override void OnDestroy()
        //{
        //    if (adapter != null)
        //        adapter.ItemClick -= OnItemClick;
        //}
    }
}