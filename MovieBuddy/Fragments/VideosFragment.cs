using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using System;
using System.Linq;

namespace MovieBuddy
{
    public class VideosFragment : MoviesFragment
    {
        public static VideosFragment NewInstance(string movieName = null, int movieId = 0, string relaseDate = null, string lang = null)
        {
            var frag1 = new VideosFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieId", movieId);
            bundle.PutString("movieName", movieName);
            bundle.PutString("relaseDate", relaseDate);
            bundle.PutString("lang", lang);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override RecyclerView.Adapter SetAdapter()
        {
            //ResetPages();
            movieAdapter = new VideosAdapter();
            movieAdapter.ItemClick += OnItemClick;
            movieAdapter.YoutubeClick += OnPosterClick;
            return movieAdapter;
        }

        private int page = 1;
        protected override bool GetData()
        {
            var date = Arguments.GetString("relaseDate");
            DateTime? relaseDate = null;
            if (!string.IsNullOrWhiteSpace(date))
                relaseDate = DateTime.Parse(date);
            var lang = Arguments.GetString("lang");
            if (MovieId > 0 && page > 1)
                return false;
            var data = MovieManager.Instance.GetVideos(MovieId, MovieName, relaseDate, lang, page++);
            if (data == null || !data.Any()) return false;

            var recyclerViewState = rv.GetLayoutManager().OnSaveInstanceState();
            movieAdapter.LoadVideos(data);
            rv.GetLayoutManager().OnRestoreInstanceState(recyclerViewState);
            return true;
        }

        protected override void OnPosterClick(object sender, int position)
        {
            base.ShowLoading();
            var videoId = (sender as VideosAdapter).videos[position].VideoId;
            var movie = (sender as VideosAdapter).videos[position].Movie;
            Intent intent = new Intent(this.Context, typeof(MovieInfoActivity));
            Bundle b = new Bundle();
            b.PutInt("movieId", movie.Id);
            b.PutString("movieName", movie.Title);
            b.PutString("movieReleaseDate", movie.ReleaseDate.HasValue ? movie.ReleaseDate.ToString() : null);
            b.PutString("movieLanguage", movie.OriginalLanguage);
            var backdrop = movie.BackdropPath;
            b.PutString("imageUrl", !string.IsNullOrWhiteSpace(backdrop) ? backdrop : movie.PosterPath);
            intent.PutExtras(b);
            StartActivity(intent);
            //Activity.OverridePendingTransition(Resource.Animation.@Side_in_right, Resource.Animation.@Side_out_left);
        }

        protected override void OnItemClick(object sender, int position)
        {
            try
            {
                DoAfterAd(() =>
                {
                    var vh = rv.FindViewHolderForAdapterPosition(position) as VideosViewHolder;
                    var videoId = (sender as VideosAdapter).videos[position].VideoId;

                    Intent intent = new Intent(this.Context, typeof(VideoViewer));
                    Bundle b = new Bundle();
                    b.PutString("videoId", videoId);
                    intent.PutExtras(b);
                    StartActivity(intent);
                });
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }

        private void DoAfterAd(Action action)
        {
            //var FinalAd = AdWrapper.ConstructFullPageAdd(Context, "ca-app-pub-3940256099942544/1033173712"); //test ad
            var FinalAd = AdWrapper.ConstructFullPageAdd(Context, "ca-app-pub-9351754143985661/8006168632");
            var intlistener = new adlistener();
            intlistener.AdFailedToLoad += () =>
            {
                action();
            };
            intlistener.AdLoaded += () =>
            {
                if (FinalAd.IsLoaded)
                {
                    FinalAd.Show();
                }
            };
            intlistener.AdClosed += () =>
            {
                action();
            };
            FinalAd.AdListener = intlistener;
            FinalAd.CustomBuild();
        }

        private void Intlistener_AdFailedToLoad()
        {
            throw new NotImplementedException();
        }

        protected override int SpanCount
        {
            get
            {
                return Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait ? 1 : 3;
            }
        }

        protected override void ResetPages() { }
    }

    public static class AdWrapper
    {
        public static InterstitialAd ConstructFullPageAdd(Context con, string UnitID)
        {
            var ad = new InterstitialAd(con)
            {
                AdUnitId = UnitID
            };
            return ad;
        }

        public static InterstitialAd CustomBuild(this InterstitialAd ad)
        {
            var requestbuilder = new AdRequest.Builder();
            ad.LoadAd(requestbuilder.Build());
            return ad;
        }
    }

    public class adlistener : AdListener
    {
        // Declare the delegate (if using non-generic pattern).
        public delegate void AdLoadedEvent();
        public delegate void AdClosedEvent();
        public delegate void AdOpenedEvent();
        public delegate void AdFailedToLoadEvent();


        // Declare the event.
        public event AdLoadedEvent AdLoaded;
        public event AdClosedEvent AdClosed;
        public event AdOpenedEvent AdOpened;
        public event AdFailedToLoadEvent AdFailedToLoad;

        public override void OnAdLoaded()
        {
            if (AdLoaded != null) this.AdLoaded();
            base.OnAdLoaded();
        }

        public override void OnAdClosed()
        {
            if (AdClosed != null) this.AdClosed();
            base.OnAdClosed();
        }
        public override void OnAdOpened()
        {
            if (AdOpened != null) this.AdOpened();
            base.OnAdOpened();
        }

        public override void OnAdFailedToLoad(int p0)
        {
            if (AdFailedToLoad != null) this.AdFailedToLoad();
            base.OnAdFailedToLoad(p0);
        }
    }
}