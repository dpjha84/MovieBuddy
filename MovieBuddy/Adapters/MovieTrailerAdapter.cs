using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Square.Picasso;
using System;
using TMDbLib.Objects.Search;

namespace MovieBuddy
{
    public class MovieTrailerAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public SearchMovie movie;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.MovieSummaryView, parent, false);
            var vh = new MovieTrailorViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as MovieTrailorViewHolder;
            vh.MovieTrailor.Text = movie.Overview;// $"{mPhotoAlbum[position].Title}\n{mPhotoAlbum[position].Date.ToString("m")}";
        }

        public override int ItemCount
        {
            get { return 1; }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}