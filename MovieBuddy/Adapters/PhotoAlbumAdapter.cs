using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace MovieBuddy
{
    public class MovieGridAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<TMDbLib.Objects.Search.SearchMovie> mPhotoAlbum;

        public MovieGridAdapter(List<TMDbLib.Objects.Search.SearchMovie> movies)
        {
            mPhotoAlbum = movies;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PhotoCardView, parent, false);
            return new PhotoViewHolder(itemView, OnClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            PhotoViewHolder vh = holder as PhotoViewHolder;
            var movie = mPhotoAlbum[position];
            Helper.SetImage(vh.Image.Context, movie.PosterPath, vh.Image, Resource.Drawable.noimage);
            vh.Name.Text = movie.Title;
            vh.Genre.Text = movie.GenreIds?.Count > 0 ? MovieManager.Instance.GetGenreText(movie.GenreIds[0]) : "";
        }

        public override int ItemCount
        {
            get { return mPhotoAlbum.Count; }
        }

        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }
}