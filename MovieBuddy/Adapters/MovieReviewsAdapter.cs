using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Square.Picasso;
using System;
using System.Collections.Generic;
using TMDbLib.Objects.Search;

namespace MovieBuddy
{
    public class MovieReviewsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<Review> reviews;

        public MovieReviewsAdapter(List<Review> reviews)
        {
            this.reviews = reviews;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Review, parent, false);
            var vh = new MovieReviewsViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MovieReviewsViewHolder vh = holder as MovieReviewsViewHolder;
            vh.ReviewerName.Text = reviews[position].Reviewer;
            //vh.ReviewerImage.Text = reviews[position].Image;
            vh.Rating.Text = reviews[position].Rating?.Replace("\"","");
            //vh.t = reviews[position].Rating;

            Context context = vh.ReviewerImage.Context;
            if (reviews[position].Image == null || string.IsNullOrWhiteSpace(reviews[position].Image.ToString()))
                Helper.SetImage(context, Resource.Drawable.NoCast, vh.ReviewerImage);
                //Glide.With(context).Load(Resource.Drawable.NoCast).Into(vh.ReviewerImage);
            else
                Helper.SetImage(context, reviews[position].Image, vh.ReviewerImage);
            //Glide.With(context).Load(reviews[position].Image).Into(vh.ReviewerImage);
            //else if (reviews[position].Image == "toi")
            //    Glide.With(context).Load(Resource.Drawable.toi2).Into(vh.ReviewerImage);
            //else if (reviews[position].Image == "imdb")
            //    Glide.With(context).Load(Resource.Drawable.imdb).Into(vh.ReviewerImage);
            //else
            //    Glide.With(context).Load($"https://image.tmdb.org/t/p/w500/{reviews[position].Image}").Into(vh.ReviewerImage);
        }

        public override int ItemCount
        {
            get { return reviews.Count; }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}