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
            Context context = vh.Image.Context;
            SetPoster(vh.Image, context, mPhotoAlbum[position].PosterPath);
        }

        private void SetPoster(ImageView img, Context context, string poster)
        {
            try
            {
                if (!string.IsNullOrEmpty(poster))
                {
                    if (poster.StartsWith("http"))
                        Helper.SetImage(context, poster, img);
                    else
                        Helper.SetImage(context, $"https://image.tmdb.org/t/p/w500/{poster}", img);
                }
                else
                    Helper.SetImage(context, Resource.Drawable.noimage, img);
            }
            catch (Exception)
            {
                Helper.SetImage(context, Resource.Drawable.noimage, img);
            }
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