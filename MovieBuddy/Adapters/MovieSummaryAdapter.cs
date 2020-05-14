using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Square.Picasso;
using System;
using TMDbLib.Objects.Search;

namespace MovieBuddy
{
    public class MovieSummaryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public string movie;

        public MovieSummaryAdapter(string movie)
        {
            this.movie = movie;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Summary, parent, false);
            MovieSummaryViewHolder vh = new MovieSummaryViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MovieSummaryViewHolder vh = holder as MovieSummaryViewHolder;
            vh.MovieSummary.Text = movie;// $"{mPhotoAlbum[position].Title}\n{mPhotoAlbum[position].Date.ToString("m")}";
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