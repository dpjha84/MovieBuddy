using System;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace MovieBuddy
{
    public class MovieSummaryViewHolder : RecyclerView.ViewHolder
    {
        public TextView MovieSummary { get; private set; }

        public MovieSummaryViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            MovieSummary = itemView.FindViewById<TextView>(Resource.Id.movieSummary);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }

    //public class MovieTrailorViewHolder : RecyclerView.ViewHolder
    //{
    //    public TextView MovieTrailor { get; private set; }

    //    public MovieTrailorViewHolder(View itemView, Action<int> listener) : base(itemView)
    //    {
    //        MovieTrailor = itemView.FindViewById<TextView>(Resource.Id.textMovieSummary);
    //        itemView.Click += (sender, e) => listener(base.LayoutPosition);
    //    }
    //}

    //public class MovieReviewsViewHolder : RecyclerView.ViewHolder
    //{
    //    public TextView ReviewerName { get; private set; }
    //    public TextView Rating { get; private set; }
    //    public ImageView ReviewerImage { get; private set; }

    //    public MovieReviewsViewHolder(View itemView, Action<int> listener) : base(itemView)
    //    {
    //        ReviewerImage = itemView.FindViewById<ImageView>(Resource.Id.ReviewerImage);
    //        ReviewerName = itemView.FindViewById<TextView>(Resource.Id.ReviewerName);
    //        Rating = itemView.FindViewById<TextView>(Resource.Id.ReviewerRating);

    //        //itemView.Click += (sender, e) => listener(base.LayoutPosition);
    //    }
    //}

    public class CastViewHolder : RecyclerView.ViewHolder
    {
        public TextView CastName { get; private set; }

        public TextView Character { get; private set; }
        public ImageView CastImage { get; private set; }

        public CastViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Character = itemView.FindViewById<TextView>(Resource.Id.Character);
            CastName = itemView.FindViewById<TextView>(Resource.Id.CastName);
            CastImage = itemView.FindViewById<ImageView>(Resource.Id.CastImage);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}