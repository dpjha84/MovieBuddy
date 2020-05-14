using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MovieBuffLib;
using System;
using System.Collections.Generic;

namespace RecyclerViewer
{
    public class GenericMoviesAdapter<TViewHolder, TResource> : RecyclerView.Adapter where TViewHolder : RecyclerView.ViewHolder, new()
    {
        public List<TmdbMovie> OtherMovies;
        int inflateResource;
        public event EventHandler<int> ItemClick;
        public GenericMoviesAdapter(int inflateResource)
        {
            this.inflateResource = inflateResource;
        }
        public override int ItemCount => throw new NotImplementedException();

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as ItemViewHolder;
            Context context = vh.Image.Context;
            SetPoster(vh.Image, context, OtherMovies[position].PosterPath);
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

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(inflateResource, parent, false);

            return new ItemViewHolder(itemView, OnClick);
        }

        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }
    public class OtherMoviesAdapter : RecyclerView.Adapter
    {        
        public event EventHandler<int> ItemClick;
        public List<TmdbMovie> OtherMovies;

        public OtherMoviesAdapter()
        {
        }

        public void SetData(List<TmdbMovie> movies)
        {
            OtherMovies = movies;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PhotoCardView1, parent, false);
            return new ItemViewHolder(itemView, OnClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ItemViewHolder vh = holder as ItemViewHolder;
            Context context = vh.Image.Context;
            SetPoster(vh.Image, context, OtherMovies[position].PosterPath);            
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
            get { return OtherMovies == null ? 0 : OtherMovies.Count; }
        }

        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }

    public class TrendingMoviesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<TmdbMovie> TrendingMovies = new List<TmdbMovie>();

        public void SetData(List<TmdbMovie> movies)
        {
            TrendingMovies = movies;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PhotoCardView2, parent, false);
            return new ItemViewHolderBig(itemView, OnClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ItemViewHolderBig vh = holder as ItemViewHolderBig;
            vh.MovieName.Text = TrendingMovies[position].Title;
            vh.Rating.Text = TrendingMovies[position].ImdbRating?.Replace("/10", "");
            if (!string.IsNullOrWhiteSpace(vh.Rating.Text))
            {
                vh.ImdbImage.SetImageResource(Resource.Drawable.imdb);
            }
            var genres = MovieManager.Instance.GetGenres(TrendingMovies[position], 1);
            vh.Genre.Text = TrendingMovies[position].ReleaseDate?.ToString("d MMM");
            if (!string.IsNullOrWhiteSpace(genres))
                vh.Genre.Text += $" | {genres}";
            Context context = vh.Image.Context;
            SetPoster(vh.Image, context, TrendingMovies[position].BackdropPath??TrendingMovies[position].PosterPath);
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
            get { return TrendingMovies == null ? 0 : TrendingMovies.Count; }
        }

        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }
}