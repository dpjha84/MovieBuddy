using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Widget;
using System;

namespace MovieBuddy
{
    public class StaticThemeActivity : ActivityBase
    {
        protected override void PreCreate()
        {
        }
    }

    [Activity(Label = "SearchActivity")]
    public class SearchActivity : StaticThemeActivity
    {
        public SearchActivity()
        {
            adRenderer = new AdRenderer();
        }

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                InitView(Resource.Layout.Search, bundle);
                adRenderer.RenderAd(FindViewById<AdView>(Resource.Id.adView));
                Title = "";

                bundle = new Bundle();
                bundle.PutString("query", Intent.GetStringExtra("query"));

                var viewPager = (ViewPager)FindViewById(Resource.Id.viewpager);
                viewPager.OffscreenPageLimit = 0;
                var tabLayout = (TabLayout)FindViewById(Resource.Id.tabs);

                var tabPagerAdapter = new SearchPagerAdapter(this, SupportFragmentManager, "");

                viewPager.Adapter = tabPagerAdapter;
                tabLayout.SetupWithViewPager(viewPager);

                for (int i = 0; i < tabLayout.TabCount; i++)
                {
                    TabLayout.Tab tab = tabLayout.GetTabAt(i);
                    tab.SetCustomView(tabPagerAdapter.GetTabView(toolbar, i));
                }

                SetupSearchView(viewPager, tabLayout);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }

        private void SetupSearchView(ViewPager mViewPager, TabLayout mTabLayout)
        {
            var searchView = FindViewById<Android.Support.V7.Widget.SearchView>(Resource.Id.searchView);
            searchView.Iconified = false;
            searchView.SetOnQueryTextListener(new QueryTextListener(this, mViewPager, mTabLayout, toolbar, SupportFragmentManager));
        }
    }
}