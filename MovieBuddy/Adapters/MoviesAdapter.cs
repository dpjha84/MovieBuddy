using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace MovieBuddy
{
    public abstract class AdapterBase : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CardViewItem, parent, false);
            return GetViewHolder(itemView, OnClick);
        }

        protected void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

        protected abstract RecyclerView.ViewHolder GetViewHolder(View view, Action<int> listener);
    }
    public class MoviesAdapter : AdapterBase
    {
        public List<TMDbLib.Objects.Search.SearchMovie> movies;

        public MoviesAdapter(List<TMDbLib.Objects.Search.SearchMovie> movies)
        {
            this.movies = movies;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MovieViewHolder vh = holder as MovieViewHolder;
            var movie = movies[position];
            Helper.SetImage(vh.Image.Context, movie.PosterPath, vh.Image, Resource.Drawable.noimage);
            vh.Name.Text = movie.Title;
            vh.Genre.Text = movie.GenreIds?.Count > 0 ? MovieManager.Instance.GetGenreText(movie.GenreIds[0]) : "";
        }

        protected override RecyclerView.ViewHolder GetViewHolder(View view, Action<int> listener)
        {
            return new MovieViewHolder(view, listener);
        }

        public override int ItemCount
        {
            get { return movies.Count; }
        }

        class MovieViewHolder : RecyclerView.ViewHolder
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