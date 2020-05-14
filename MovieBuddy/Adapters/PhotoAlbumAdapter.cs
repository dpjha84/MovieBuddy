using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Square.Picasso;
using System;
using System.Collections.Generic;
//using TMDbLib.Objects.Search;

namespace MovieBuddy
{
    public class MovieGridAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<TmdbMovie> mPhotoAlbum;
        private bool mainPage = false;
        private bool trailerView = false;

        public MovieGridAdapter(List<TmdbMovie> movies, bool mainPage = false, bool trailerView = false)
        {
            mPhotoAlbum = movies;
            this.mainPage = mainPage;
            this.trailerView = trailerView;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = trailerView ?
                    LayoutInflater.From(parent.Context).Inflate(Resource.Layout.TrailerGridView, parent, false):
                    LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PhotoCardView, parent, false);
            //if (trailerView)
            //    return new DashboardViewHolder(itemView, OnClick, null);
                return new PhotoViewHolder(itemView, OnClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //if (dashBoard)
            //{
            //    DashboardViewHolder vh = holder as DashboardViewHolder;
            //    Context context = vh.Image.Context;
            //    SetPoster(vh.Image, context, mPhotoAlbum[position].PosterPath);
            //}
            //else
            //{
                PhotoViewHolder vh = holder as PhotoViewHolder;

                // Set the ImageView and TextView in this ViewHolder's CardView 
                // from this position in the photo album:
                //ImageService.Instance.LoadUrl(urlToImage).Into(_imageView);
                //string url = "http://i.imgur.com/DvpvklR.png";// 
                //string url = "https://images-na.ssl-images-amazon.com/images/M/MV5BMjAxMzY3NjcxNF5BMl5BanBnXkFtZTcwNTI5OTM0Mw@@._V1_UX182_CR0,0,182,268_AL__QL50.jpg";// SetImageResource (mPhotoAlbum[position].PhotoID);
                //vh.Caption.Text = mPhotoAlbum[position].Title;
                //if (mainPage)
                //{
                //    vh.Date.Text = mPhotoAlbum[position].ReleaseDate?.ToString("d MMM");
                //    vh.Genre.Text = MovieManager.Instance.GetGenres(mPhotoAlbum[position], 2);
                //var genres = "";
                //    if (mPhotoAlbum[position]?.Genres != null)
                //    {
                //        foreach (var genre in mPhotoAlbum[position].Genres)
                //        {
                //            genres += genre.Name + ", ";
                //        }
                //        if (!string.IsNullOrWhiteSpace(genres) && genres.EndsWith(", "))
                //            genres = genres.Substring(0, genres.Length - 2);
                //        vh.Genre.Text = MovieManager.Instance.GetGenres(mPhotoAlbum[position], 2);
                //    }


                //    if (!trailerView && mPhotoAlbum[position].ReleaseDate <= DateTime.Now)
                //    {
                //        var key = (mPhotoAlbum[position].Id == 0) ? mPhotoAlbum[position].Title.Replace(" ", "") : mPhotoAlbum[position].Id.ToString();
                //        var imdbData = MovieManager.Instance.GetImdbRating(key)?.Replace("\"", "")?.Replace("/10", "");
                //        if (!string.IsNullOrWhiteSpace(imdbData))
                //        {
                //            vh.ImdbRating.Text = imdbData;
                //            vh.ImdbImage.SetImageResource(Resource.Drawable.imdb);
                //        }
                //    }
                //}
                Context context = vh.Image.Context;
                SetPoster(vh.Image, context, mPhotoAlbum[position].PosterPath);
            //}
        }

        private void SetPoster(ImageView img, Context context, string poster)
        {
            try
            {
                if (!string.IsNullOrEmpty(poster))
                {
                    if (poster.StartsWith("http"))
                        Helper.SetImage(context, poster, img);
                    //Glide.With(context).Load(poster).Into(vh.Image);
                    else
                        Helper.SetImage(context, $"https://image.tmdb.org/t/p/w500/{poster}", img);
                }
                else
                    Helper.SetImage(context, Resource.Drawable.noimage, img);
                //Glide.With(context).Load(Resource.Drawable.noimage).Into(vh.Image);
            }
            catch (Exception)
            {
                Helper.SetImage(context, Resource.Drawable.noimage, img);
                //Glide.With(context).Load(Resource.Drawable.noimage).Into(vh.Image);
                //throw;
            }
        }

        public override int ItemCount
        {
            get { return mPhotoAlbum.Count; }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

    }
}