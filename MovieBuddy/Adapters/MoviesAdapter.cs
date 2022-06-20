using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using MovieBuddy.Activities;
using System;
using System.Collections.Generic;
using TSearchMovie = TMDbLib.Objects.Search.SearchMovie;

namespace MovieBuddy
{
    public abstract class AdapterBase : RecyclerView.Adapter
    {
        //public event EventHandler<int> ItemClick;
        public List<ImageView> ImageViewsToClean = new List<ImageView>();

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CardViewItem, parent, false);
            View mainView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.fragment_blank, parent, false);
            return GetViewHolder(itemView);
        }

        protected abstract RecyclerView.ViewHolder GetViewHolder(View view, View contentView = null, View parentView = null);
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

        protected override RecyclerView.ViewHolder GetViewHolder(View view, View contentView = null, View parentView = null)
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
            View contentView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.fragment_blank, parent, false);
            View parentView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Main, parent, false);
            return GetViewHolder(itemView, contentView, parentView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            VideosViewHolder vh = holder as VideosViewHolder;
            var videoId = videos[position];
            //Helper.SetImage(vh.Thumbnail.Context, $"https://img.youtube.com/vi/{videoId}/0.jpg", vh.Thumbnail, Resource.Drawable.noimage, true);
            String myYouTubeVideoUrl = $"https://www.youtube.com/embed/{videoId}";

            
            //Helper.SetImage(vh.Thumbnail.Context, $"https://img.youtube.com/vi/{videoId}/0.jpg", vh.Thumbnail, Resource.Drawable.noimage, true);
            var url = $"<iframe width=\"100%\" height=\"600\" src=\"{myYouTubeVideoUrl}\" frameborder=\"0\" allowfullscreen/>";
            //var linearLayout = FindViewById<CardView>(Resource.Id.linearLayout);
            //var contentLayout = FindViewById<FrameLayout>(Resource.Id.contentLayout);
            //WebView webView = findViewById(R.id.mWebView);
            WebSettings webSettings = vh.WebView.Settings;
            webSettings.JavaScriptEnabled = true;
            webSettings.CacheMode = CacheModes.CacheElseNetwork;
            //vh.WebView.SetWebChromeClient(new FullScreenClient(vh.ParentLayout, vh.ContentLayout));
            //webSettings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.Normal);
            webSettings.LoadWithOverviewMode = true;
            webSettings.UseWideViewPort = true;
            vh.WebView.LoadData(url, "text/html", "utf-8");
        }

        protected override RecyclerView.ViewHolder GetViewHolder(View view, View contentView, View parentView)
        {
            return new VideosViewHolder(view, contentView, parentView, OnClick);
        }

        public override int ItemCount
        {
            get { return videos.Count; }
        }

        class VideosViewHolder : RecyclerView.ViewHolder
        {
            public WebView WebView { get; private set; }
            public FrameLayout ContentLayout { get; private set; }
            public CardView ParentLayout { get; private set; }
            public ImageView Thumbnail { get; private set; }
            public ImageView PlayButton { get; private set; }
            public TextView Genre { get; private set; }

            public VideosViewHolder(View itemView, View contentView, View parentView, Action<int> listener) : base(itemView)
            {
                //Helper.SetImage(this.Context, $"https://img.youtube.com/vi/{trailerId}/0.jpg", backdropImage, Resource.Drawable.noimage, true);
                WebView = itemView.FindViewById<WebView>(Resource.Id.mWebView);
                ContentLayout = itemView.FindViewById<FrameLayout>(Resource.Id.frameLayout2);

                //View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CardViewItem, parent, false);

                ParentLayout = itemView.FindViewById<CardView>(Resource.Id.videoCardView);
                //Thumbnail = itemView.FindViewById<ImageView>(Resource.Id.mediaPreview);
                //PlayButton = itemView.FindViewById<ImageView>(Resource.Id.playButton);
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }

    public class CastMoviesAdapter : MoviesAdapter
    {
        protected override string GetExtraText(TSearchMovie movie) => movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.Year.ToString() : "";
    }

    public class ImdbMoviesAdapter : MoviesAdapter
    {
        protected override string GetExtraText(TSearchMovie movie) => movie.OriginalTitle;
    }

    public class TmdbTopRatedMoviesAdapter : MoviesAdapter
    {
        protected override string GetExtraText(TSearchMovie movie) => $"{movie.VoteAverage*10}%";
    }
}