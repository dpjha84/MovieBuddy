using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace MovieBuddy
{
    public class VideosAdapter : MoviesAdapter
    {
        public List<VideoData> videos;

        public VideosAdapter()
        {
            videos = new List<VideoData>();
        }

        protected override void AddVideosToCollection(List<VideoData> data)
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
            var videoData = videos[position];

            Helper.SetImage(vh.Thumbnail.Context, $"https://img.youtube.com/vi/{videoData.VideoId}/0.jpg", vh.Thumbnail, Resource.Drawable.noimage, true);
            if (videoData.Movie == null)
            {
                vh.Poster.Visibility = ViewStates.Gone;
                vh.MovieName.Visibility = ViewStates.Gone;
            }
            else
            {
                Helper.SetImage(vh.Poster.Context, videoData.Movie.PosterPath, vh.Poster, Resource.Drawable.noimage);
                vh.MovieName.Text = videoData.Movie.Title;
            }
        }

        protected override RecyclerView.ViewHolder GetViewHolder(View view)
        {
            return new VideosViewHolder(view, OnClick, OnPosterClick);
        }

        public override int ItemCount
        {
            get { return videos.Count; }
        }
    }

    public class VideosViewHolder : RecyclerView.ViewHolder
    {
        public WebView WebView { get; private set; }
        //public FrameLayout ContentLayout { get; private set; }
        public CardView ParentLayout { get; private set; }
        public ImageView Thumbnail { get; private set; }
        public ImageView Poster { get; private set; }
        public ImageView PlayButton { get; private set; }
        public TextView MovieName { get; private set; }
        //public ImageView YoutubeButton { get; private set; }
        //public ImageView YoutubeButton1 { get; private set; }
        //public TextView Genre { get; private set; }

        public VideosViewHolder(View itemView, Action<int> listener, Action<int> posterClickListener) : base(itemView)
        {
            //Helper.SetImage(this.Context, $"https://img.youtube.com/vi/{trailerId}/0.jpg", backdropImage, Resource.Drawable.noimage, true);
            //WebView = itemView.FindViewById<WebView>(Resource.Id.mWebView);
            //ContentLayout = itemView.FindViewById<FrameLayout>(Resource.Id.frameLayout2);

            //View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CardViewItem, parent, false);

            ParentLayout = itemView.FindViewById<CardView>(Resource.Id.videoCardView);
            if (DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
                ParentLayout.LayoutParameters.Height = (int)DeviceDisplay.MainDisplayInfo.Height / 2;
            Thumbnail = itemView.FindViewById<ImageView>(Resource.Id.mediaPreview);
            Poster = itemView.FindViewById<ImageView>(Resource.Id.mediaPreview1);
            MovieName = itemView.FindViewById<TextView>(Resource.Id.movieName);
            PlayButton = itemView.FindViewById<ImageView>(Resource.Id.playButton);
            //YoutubeButton = itemView.FindViewById<ImageView>(Resource.Id.youtubeButton);
            //YoutubeButton1 = itemView.FindViewById<ImageView>(Resource.Id.youtubeButton1);
            PlayButton.Click += (sender, e) => listener(base.LayoutPosition);
            Poster.Click += (sender, e) => posterClickListener(base.LayoutPosition);
            //YoutubeButton.Click += (sender, e) => youtubeClickListener(base.LayoutPosition);
            //YoutubeButton1.Click += (sender, e) => youtubeClickListener(base.LayoutPosition);
        }
    }
}