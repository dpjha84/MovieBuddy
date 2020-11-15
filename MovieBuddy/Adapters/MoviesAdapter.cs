using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace MovieBuddy
{
    public abstract class AdapterBase : RecyclerView.Adapter
    {
        //public event EventHandler<int> ItemClick;
        public List<ImageView> ImageViewsToClean = new List<ImageView>();

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CardViewItem, parent, false);
            return GetViewHolder(itemView);
        }

        protected abstract RecyclerView.ViewHolder GetViewHolder(View view);
    }

    public interface IEntity
    {

    }

    public interface IPaginatedAdapter<T>
    {
        void LoadData(List<T> data);
    }

    public interface IClickableAdapter
    {
        event EventHandler<int> ItemClick;

        void OnClick(int position);
    }

    public interface IClickableWithPagingAdapter<T> : IClickableAdapter, IPaginatedAdapter<T>
    {
    }

    public abstract class ClickableWithPagingAdapter<T> : AdapterBase, IClickableWithPagingAdapter<T>
    {
        public event EventHandler<int> ItemClick;
        public virtual void LoadData(List<T> data)
        {
            AddToCollection(data);
            NotifyDataSetChanged();
        }
        public virtual void LoadVideos(List<string> data)
        {
            AddVideosToCollection(data);
            NotifyDataSetChanged();
        }

        public void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }


        protected abstract void AddToCollection(List<T> data);

        protected virtual void AddVideosToCollection(List<string> data) { }
    }

    public abstract class ClickableAdapter : AdapterBase, IClickableAdapter
    {
        public event EventHandler<int> ItemClick;

        public void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }
    }

    public abstract class PaginatedAdapter<T> : AdapterBase, IPaginatedAdapter<T>
    {
        public void LoadData(List<T> data)
        {
            AddToCollection(data);
            NotifyDataSetChanged();
        }

        protected virtual void AddToCollection(List<T> data) { }
    }

    public class MoviesAdapter : ClickableWithPagingAdapter<TMDbLib.Objects.Search.SearchMovie>
    {
        public List<TMDbLib.Objects.Search.SearchMovie> movies;

        public MoviesAdapter()
        {
            movies = new List<TMDbLib.Objects.Search.SearchMovie>();
        }

        protected override void AddToCollection(List<TMDbLib.Objects.Search.SearchMovie> data)
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
            Helper.SetImage(vh.Image.Context, movie.PosterPath, vh.Image, Resource.Drawable.noimage);
            vh.Name.Text = movie.Title;
            vh.Genre.Text = movie.GenreIds?.Count > 0 ? MovieManager.Instance.GetGenreText(movie.GenreIds[0]) : "";
        }

        protected override RecyclerView.ViewHolder GetViewHolder(View view)
        {
            return new MovieViewHolder(view, OnClick);
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

    public class VideosAdapter : MoviesAdapter
    {
        public List<string> videos;

        public VideosAdapter()
        {
            videos = new List<string>();
        }

        protected override void AddVideosToCollection(List<string> data)
        {
            videos.AddRange(data);
        }

        public override long GetItemId(int position)
        {
            return videos[position].GetHashCode();
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.video, parent, false);
            return GetViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            VideosViewHolder vh = holder as VideosViewHolder;
            var videoId = videos[position];
            Helper.SetImage(vh.Thumbnail.Context, $"https://img.youtube.com/vi/{videoId}/0.jpg", vh.Thumbnail, Resource.Drawable.noimage, true);
        }

        protected override RecyclerView.ViewHolder GetViewHolder(View view)
        {
            return new VideosViewHolder(view, OnClick);
        }

        public override int ItemCount
        {
            get { return videos.Count; }
        }

        class VideosViewHolder : RecyclerView.ViewHolder
        {
            public ImageView Thumbnail { get; private set; }
            public ImageView PlayButton { get; private set; }
            public TextView Genre { get; private set; }

            public VideosViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                //Helper.SetImage(this.Context, $"https://img.youtube.com/vi/{trailerId}/0.jpg", backdropImage, Resource.Drawable.noimage, true);

                Thumbnail = itemView.FindViewById<ImageView>(Resource.Id.mediaPreview);
                PlayButton = itemView.FindViewById<ImageView>(Resource.Id.playButton);
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}