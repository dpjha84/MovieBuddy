using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Jaeger;
using System;

namespace MovieBuddy
{
    [Activity(Label = "MovieInfoActivity")]
    public class MovieInfoActivity : ActivityBase
    {
        int movieId;

        public MovieInfoActivity()
        {
            statusBar = new TransparentStatusBarSetter();
        }

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                InitView(Resource.Layout.MovieInfoView, bundle);
                //statusBar.SetTransparent(this);

                var mAdView = FindViewById<AdView>(Resource.Id.adView);
                var adRequest = new AdRequest.Builder().Build();
                mAdView.LoadAd(adRequest);

                string movieName = Intent.GetStringExtra("movieName");
                string movieLang = Intent.GetStringExtra("movieLanguage");
                var releaseDate = Intent.GetStringExtra("movieReleaseDate");
                //int releaseYear = 0;
                //if (!string.IsNullOrWhiteSpace(releaseDate))
                //    releaseYear = DateTime.Parse(releaseDate).Year;
                movieId = Intent.GetIntExtra("movieId", 0);
                string castName = Intent.GetStringExtra("castName");
                int castId = Intent.GetIntExtra("castId", 0);
                bool isCast = Intent.GetBooleanExtra("isCast", false);

                bundle = new Bundle();
                bundle.PutInt("movieId", movieId);
                bundle.PutString("name", movieName);
                bundle.PutInt("castId", castId);
                bundle.PutBoolean("isCast", isCast);

                Title = movieName;

                var fab = FindViewById<Refractored.Fab.FloatingActionButton>(Resource.Id.fab);
                fab.Visibility = ViewStates.Visible;
                fab.Click += (sender, args) =>
                {
                    //Toast.MakeText(this, "FAB Clicked!", ToastLength.Short).Show();
                    ShowOptionsDialog();
                };

                var image = FindViewById<ImageView>(Resource.Id.backdrop);
                Helper.SetImage(this, Intent.GetStringExtra("imageUrl"), image, Resource.Drawable.noimage);
                image.Click += Image_Click;

                var mViewPager = (ViewPager)FindViewById(Resource.Id.viewpager);
                mViewPager.OffscreenPageLimit = 0;
                var mTabLayout = (TabLayout)FindViewById(Resource.Id.tabs);
                var tabPagerAdapter = new MoviePagerAdapter(this, SupportFragmentManager, movieName, movieId, Intent.GetStringExtra("imageUrl"), releaseDate, movieLang);

                mViewPager.Adapter = tabPagerAdapter;
                mTabLayout.SetupWithViewPager(mViewPager);

                for (int i = 0; i < mTabLayout.TabCount; i++)
                {
                    TabLayout.Tab tab = mTabLayout.GetTabAt(i);
                    tab.SetCustomView(tabPagerAdapter.GetTabView(toolbar, i));
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }
        private void Image_Click(object sender, EventArgs e)
        {
            Bundle b = new Bundle();
            Intent intent = new Intent(this, typeof(ImageViewer));
            b.PutString("url", Intent.GetStringExtra("imageUrl"));
            intent.PutExtras(b);
            StartActivity(intent);
        }

        private void ShowOptionsDialog()
        {
            var dialogView = LayoutInflater.Inflate(Resource.Layout.movie_options, null);
            Android.App.AlertDialog alertDialog;
            //var adap = new ArrayAdapter<string>(this, Resource.Layout.select_dialog_singlechoice_material);
            using (var dialog = new Android.App.AlertDialog.Builder(this))
            {
                dialog.SetTitle("Choose Action on this movie");
                dialog.SetView(dialogView);
                dialog.SetNegativeButton("Cancel", (s, a) => { });
                alertDialog = dialog.Create();
            }
            var items = new string[] { "Add to Favourite", "Add to Already Watched", "Add to Watch List" };
            if (Globals.StarredMovies.Contains(movieId))
            {
                items[0] = "Remove from Favourite";
            }
            if (Globals.WatchedMovies.Contains(movieId))
            {
                items[1] = "Remove from Already Watched";
            }
            if (Globals.ToWatchMovies.Contains(movieId))
            {
                items[2] = "Remove from 'To Watch' List";
            }
            var list = (ListView)dialogView.FindViewById(Resource.Id.listMovieOptions);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, items);
            list.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
            {                
                if (e.Position == 0)
                {
                    if(items[0] == "Add to Favourite")
                        Globals.AddToStarredMovies(movieId);
                    else
                        Globals.RemoveFromStarredMovies(movieId);
                }
                else if (e.Position == 1)
                {
                    if (items[1] == "Add to Already Watched")
                        Globals.AddToWatchedMovies(movieId);
                    else
                        Globals.RemoveFromWatchedMovies(movieId);
                }
                else
                {
                    if (items[2] == "Add to Watch List")
                        Globals.AddToWatchMovies(movieId);
                    else
                        Globals.RemoveFromToWatchMovies(movieId);
                }
                alertDialog.Dismiss();
            };
            list.Adapter = adapter;
            alertDialog.Show();
        }
    }
}