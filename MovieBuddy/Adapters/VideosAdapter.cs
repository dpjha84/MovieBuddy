using Android.Support.V7.Widget;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace MovieBuddy
{
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
            string myYouTubeVideoUrl = $"https://www.youtube.com/embed/{videoId}";
            Helper.SetImage(vh.Thumbnail.Context, $"https://img.youtube.com/vi/{videoId}/0.jpg", vh.Thumbnail, Resource.Drawable.noimage, true);
            
            //var url = $"<iframe width=\"100%\" height=\"600\" src=\"{myYouTubeVideoUrl}\" frameborder=\"0\" allowfullscreen/>";
            //WebSettings webSettings = vh.WebView.Settings;
            //webSettings.JavaScriptEnabled = true;
            //webSettings.CacheMode = CacheModes.CacheElseNetwork;
            //if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
            //    vh.WebView.SetLayerType(LayerType.Hardware, null);
            //else
            //    vh.WebView.SetLayerType(LayerType.Software, null);

            ////vh.WebView.SetWebChromeClient(new FullScreenClient(vh.ParentLayout, vh.ContentLayout));
            //vh.WebView.SetWebChromeClient(new WebChromeClient());
            //webSettings.SetLayoutAlgorithm(WebSettings.LayoutAlgorithm.NarrowColumns);
            //webSettings.SavePassword = true;
            //webSettings.SaveFormData = true;
            //webSettings.SetEnableSmoothTransition(true);
            //webSettings.LoadWithOverviewMode = true;
            //webSettings.UseWideViewPort = true;
            //webSettings.SetRenderPriority(WebSettings.RenderPriority.High);
            //webSettings.SetAppCacheEnabled(true);
            //vh.WebView.ScrollBarStyle = ScrollbarStyles.InsideOverlay;
            //webSettings.DomStorageEnabled = true;
            //vh.WebView.LoadData(url, "text/html", "utf-8");
        }

        protected override RecyclerView.ViewHolder GetViewHolder(View view, View contentView, View parentView)
        {
            return new VideosViewHolder(view, contentView, parentView, OnClick);
        }

        public override int ItemCount
        {
            get { return videos.Count; }
        }
    }

    public class VideosViewHolder : RecyclerView.ViewHolder
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
            Thumbnail = itemView.FindViewById<ImageView>(Resource.Id.mediaPreview);
            PlayButton = itemView.FindViewById<ImageView>(Resource.Id.playButton);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}