using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Views;
using System.Diagnostics;
using Android.Widget;
using Android.Gms.Ads;
using System;
using Android.Content;

namespace MovieBuddy
{
    [Activity (Label = "Movie Buddy", MainLauncher = false, Icon = "@drawable/icon")]
	public class MainActivity : AppCompatActivity
    {
        protected AdView mAdView;
        DrawerLayout drawerLayout;
        NavigationView navigationView;
        IMenuItem previousItem;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            try
            {
                //MovieManager.Init(new LocalDataProvider(), false);
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.Main);

                mAdView = FindViewById<AdView>(Resource.Id.adView);
                var adRequest = new AdRequest.Builder().Build();
                mAdView.LoadAd(adRequest);

                var toolbar = SetupToolbar();

                SetupTabbedView(toolbar);

                SetupNavigationDrawer(savedInstanceState);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }

        private Android.Support.V7.Widget.Toolbar SetupToolbar()
        {
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
            }

            return toolbar;
        }

        private void SetupTabbedView(Android.Support.V7.Widget.Toolbar toolbar)
        {
            ViewPager viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            HomePagerAdapter pagerAdapter = new HomePagerAdapter(this, SupportFragmentManager);
            viewPager.Adapter = pagerAdapter;
            

            var tabs = FindViewById<TabLayout>(Resource.Id.tab_layout);
            tabs.SetupWithViewPager(viewPager);

            for (int i = 0; i < tabs.TabCount; i++)
            {
                TabLayout.Tab tab = tabs.GetTabAt(i);
                tab.SetCustomView(pagerAdapter.GetTabView(toolbar, i));
            }
            //pagerAdapter.GetItem(1);
        }

        private void SetupNavigationDrawer(Bundle savedInstanceState)
        {
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            //Set hamburger items menu
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);

            //setup navigation view
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

            //handle navigation
            navigationView.NavigationItemSelected += (sender, e) =>
            {
                if (previousItem != null)
                    previousItem.SetChecked(false);

                navigationView.SetCheckedItem(e.MenuItem.ItemId);

                previousItem = e.MenuItem;

                switch (e.MenuItem.ItemId)
                {
                    case Resource.Id.nav_home_1:
                        ListItemClicked(0);
                        break;
                    //case Resource.Id.nav_home_2:
                    //    ListItemClicked(1);
                    //    break;
                }
                drawerLayout.CloseDrawers();
            };

            //if first time you will want to go ahead and click first item.
            //if (savedInstanceState == null)
            //{
            //    navigationView.SetCheckedItem(Resource.Id.nav_home_1);
            //    ListItemClicked(0);
            //}
        }
        
        private void ListItemClicked(int position)
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.dpjha.moviebuddy"));
            StartActivity(intent);
            //this way we don't load twice, but you might want to modify this a bit.
            //if (position == oldPosition)
            //    return;

            //oldPosition = position;

            //Android.Support.V4.App.Fragment fragment = null;
            //switch (position)
            //{
            //    case 0:
            //        fragment = Fragment1.NewInstance();
            //        break;
            //    case 1:
            //        fragment = Fragment2.NewInstance();
            //        break;
            //}

            //SupportFragmentManager.BeginTransaction()
            //    .Replace(Resource.Id.main_layout, fragment)
            //    .Commit();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }    
}
