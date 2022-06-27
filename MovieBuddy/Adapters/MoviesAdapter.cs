using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using TSearchMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy
{
    public class MoviesAdapter : ClickableWithPagingAdapter<TSearchMovie>
    {
        public List<TSearchMovie> movies;

        public MoviesAdapter()
        {
            movies = new List<TMDbLib.Objects.Search.SearchMovie>();
        }

        protected override void AddToCollection(List<TSearchMovie> data)
        {
            movies.AddRange(data);
        }

        public override long GetItemId(int position)
        {
            return movies[position].GetHashCode();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MovieViewHolder vh = holder as MovieViewHolder;
            //ImageViewsToClean.Add(vh.Image);
            var movie = movies[position];
            //Helper.SetImage(vh.Image.Context, movie.PosterPath, vh.Image, Resource.Drawable.noimage);
            SetImage(vh, movie);
            vh.Name.Text = movie.Title;
            vh.Genre.Text = GetExtraText(movie);
        }

        protected virtual void SetImage(MovieViewHolder vh, TSearchMovie movie)
        {
            Helper.SetImage(vh.Image.Context, movie.PosterPath, vh.Image, Resource.Drawable.noimage);
        }

        protected virtual string GetExtraText(TSearchMovie movie) => movie.GenreIds?.Count > 0 ? MovieManager.Instance.GetGenreText(movie.GenreIds[0]) : "";

        protected override RecyclerView.ViewHolder GetViewHolder(View view)
        {
            return new MovieViewHolder(view, OnClick);
        }

        public override int ItemCount
        {
            get { return movies.Count; }
        }

        protected class MovieViewHolder : RecyclerView.ViewHolder
        {
            public ImageView Image { get; private set; }
            public TextView Name { get; private set; }
            public TextView Genre { get; private set; }

            public MovieViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                Image = itemView.FindViewById<ImageView>(Resource.Id.Image);
                Name = itemView.FindViewById<TextView>(Resource.Id.Name);
                Genre = itemView.FindViewById<TextView>(Resource.Id.Description);
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}