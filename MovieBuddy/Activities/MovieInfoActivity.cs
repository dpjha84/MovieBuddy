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
                statusBar.SetTransparent(this);

                string movieName = Intent.GetStringExtra("movieName");
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
                fab.Click += (sender, args) =>
                {
                    //Toast.MakeText(this, "FAB Clicked!", ToastLength.Short).Show();
                    ShowOptionsDialog();
                };

                Helper.SetImage(this, Intent.GetStringExtra("imageUrl"), FindViewById<ImageView>(Resource.Id.backdrop), Resource.Drawable.noimage);

                var mViewPager = (ViewPager)FindViewById(Resource.Id.viewpager);
                mViewPager.OffscreenPageLimit = 0;
                var mTabLayout = (TabLayout)FindViewById(Resource.Id.tabs);
                var tabPagerAdapter = new MoviePagerAdapter(this, SupportFragmentManager, movieName, movieId, Intent.GetStringExtra("imageUrl"));

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

        private void ShowOptionsDialog()
        {
            var dialogView = LayoutInflater.Inflate(Resource.Layout.movie_options, null);
            Android.App.AlertDialog alertDialog;
            using (var dialog = new Android.App.AlertDialog.Builder(this))
            {
                dialog.SetTitle("Choose Action on this movie");
                dialog.SetView(dialogView);
                dialog.SetNegativeButton("Cancel", (s, a) => { });
                alertDialog = dialog.Create();
            }
            var list = (ListView)dialogView.FindViewById(Resource.Id.listMovieOptions);
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, Resources.GetTextArray(Resource.Array.movie_options_array));
            list.ItemClick += (object sender, Android.Widget.AdapterView.ItemClickEventArgs e) =>
            {
                if (e.Position == 0)
                {
                    Globals.AddToStarredMovies(movieId);
                }
                else
                {
                    string selectedFromList = list.GetItemAtPosition(e.Position).ToString();
                    Toast.MakeText(this, selectedFromList, ToastLength.Long).Show();
                }
            };
            list.Adapter = adapter;
            alertDialog.Show();
        }
    }
}