using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;

namespace MovieBuddy
{


    [Activity(Label = "Movie Buddy", MainLauncher = false, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {
        protected AdView mAdView;
        DrawerLayout drawerLayout;
        NavigationView navigationView;
        IMenuItem previousItem;
        Android.Support.V7.Widget.Toolbar toolbar;
        HomePagerAdapter pagerAdapter;
        ViewPager viewPager;
        TabLayout tabs;
        Dialog dialog;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            Xamarin.Essentials.Platform.Init(this, bundle);

            mAdView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Movie Buddy";
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            SetupTabbedView(toolbar);

            SetupNavigationDrawer(bundle);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void SetupTabbedView(Android.Support.V7.Widget.Toolbar toolbar)
        {
            viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            pagerAdapter = new HomePagerAdapter(this, SupportFragmentManager);
            viewPager.Adapter = pagerAdapter;

            tabs = FindViewById<TabLayout>(Resource.Id.tab_layout);
            tabs.SetupWithViewPager(viewPager);

            for (int i = 0; i < tabs.TabCount; i++)
            {
                TabLayout.Tab tab = tabs.GetTabAt(i);
                tab.SetCustomView(pagerAdapter.GetTabView(toolbar, i));
            }
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
                        Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.dpjha.moviebuddy"));
                        StartActivity(intent);
                        break;
                    case Resource.Id.share:
                        Intent sendIntent = new Intent();
                        sendIntent.SetAction(Intent.ActionSend);
                        sendIntent.PutExtra(Intent.ExtraText,
                            "Hey check out this app at: https://play.google.com/store/apps/details?id=com.dpjha.moviebuddy");
                        sendIntent.SetType("text/plain");
                        StartActivity(sendIntent);
                        break;
                    case Resource.Id.celebs:
                        Intent intent1 = new Intent(this, typeof(PopularPersonActivity));
                        StartActivity(intent1);
                        break;
                    case Resource.Id.explore:
                        StartActivity(new Intent(this, typeof(ExploreActivity)));
                        break;
                }
                drawerLayout.CloseDrawers();
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(GravityCompat.Start);
                    break;
                case Resource.Id.filter:
                    ShowRadioButtonDialog();
                    break;
                case Resource.Id.search:
                    Intent intent = new Intent(this, typeof(SearchActivity));
                    Bundle b = new Bundle();
                    b.PutString("query", "john");
                    intent.PutExtras(b);
                    StartActivity(intent);
                    break;
                default:
                    Toast.MakeText(this, item.TitleFormatted + ": " + "Overflow", ToastLength.Long).Show();
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void ShowRadioButtonDialog()
        {
            dialog = new Dialog(this);
            dialog.SetContentView(Resource.Layout.radiobutton_dialog);
            dialog.SetCancelable(true);

            RadioGroup rg = (RadioGroup)dialog.FindViewById(Resource.Id.radio_group);
            foreach (var lang in Globals.LanguageMap)
            {
                RadioButton rb = new RadioButton(this);
                rb.SetPadding(0, 20, 0, 20);
                if (lang.Key == Globals.SelectedLanguage)
                    rb.Checked = true;
                rb.Click += Rb_Click;
                rb.SetText(lang.Key, TextView.BufferType.Normal);
                rg.AddView(rb);
            }
            dialog.Show();
        }

        private void Rb_Click(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            Globals.SelectedLanguage = rb.Text;
            var prevSelectedTab = tabs.SelectedTabPosition;
            SetupTabbedView(toolbar);
            tabs.GetTabAt(prevSelectedTab).Select();
            dialog.Dismiss();
        }

        private void ListItemClicked(int position)
        {
            Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.dpjha.moviebuddy"));
            StartActivity(intent);
        }
    }
}
